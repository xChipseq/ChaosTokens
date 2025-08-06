using System.Linq;
using AmongUs.GameOptions;
using ChaosTokens.Modifiers;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Utilities;
using UnityEngine;

namespace ChaosTokens;

public static class TokenEvents
{
    private static float DoubleTokenChance { get; set; } = OptionGroupSingleton<ChaosTokensOptions>.Instance.InitialDoubleTokenChance;
    
    [RegisterEvent]
    public static void HandleVotesEventHandler(HandleVoteEvent @event)
    {
        var target = MiscUtils.PlayerById(@event.TargetId);
        if (target != null && target.HasModifier<TokenDeath>())
        {
            @event.Cancel();
            return;
        }
        
        if (@event.VoteData.Owner.HasModifier<TokenVotes>())
        {
            @event.VoteData.SetRemainingVotes(0);

            for (var i = 0; i < Random.RandomRangeInt(2, 4); i++)
            {
                @event.VoteData.VoteForPlayer(@event.TargetId);
            }

            @event.Cancel();
        }
    }
    
    [RegisterEvent]
    public static void VotingCompleteEventHandler(VotingCompleteEvent @event)
    {
        ModifierUtils.GetPlayersWithModifier<TokenDeath>().Do(player =>
        {
            player.CustomMurder(player, MurderResultFlags.Succeeded, createDeadBody: false, showKillAnim: false);
            DeathHandlerModifier.UpdateDeathHandler(player, "Fate");
        });

        try
        {
            MeetingHud.Instance.playerStates.Do(x => x.transform.FindChild("TokenDeathIcon").gameObject.DestroyImmediate());
        }
        catch {} // don't care
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.HasModifier<TokenProtection>())
                player.RemoveModifier<TokenProtection>();
            if (player.HasModifier<TokenVotes>())
                player.RemoveModifier<TokenVotes>();
            if (player.HasModifier<TokenRandomModifier>())
                player.RemoveModifier<TokenRandomModifier>();
            if (player.HasModifier<TokenTransparent>())
                player.RemoveModifier<TokenTransparent>();
        }
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!OptionGroupSingleton<ChaosTokensOptions>.Instance.EnableChaosTokens)
        {
            return;
        }
        if (!AmongUsClient.Instance.AmHost)
        {
            return;
        }
        if (@event.TriggeredByIntro && !OptionGroupSingleton<ChaosTokensOptions>.Instance.TokensEnabledFirstRound)
        {
            return;
        }

        int min = (int)OptionGroupSingleton<ChaosTokensOptions>.Instance.TokensMin;
        int max = (int)OptionGroupSingleton<ChaosTokensOptions>.Instance.TokensMax;

        var potentialPlayers = Helpers.GetAlivePlayers();
        potentialPlayers.Shuffle();
        int tokensToHandle = Mathf.Clamp(Random.RandomRangeInt(min, max), 0, potentialPlayers.Count);

        foreach (var player in potentialPlayers)
        {
            int tokens = 1;
            if (Random.RandomRangeInt(1, 100) < DoubleTokenChance && tokensToHandle > 1)
            {
                tokens = 2;
            }

            if (player.TryGetModifier<ChaosTokenModifier>(out var modifier))
            {
                modifier.IncreaseTokens(tokens);
            }
            else
            {
                player.RpcAddModifier<ChaosTokenModifier>(tokens, true);
            }
            
            tokensToHandle -= tokens;
            if (tokensToHandle <= 0)
            {
                break;
            }
        }
        
        DoubleTokenChance = Mathf.Clamp(DoubleTokenChance + OptionGroupSingleton<ChaosTokensOptions>.Instance.DoubleTokenIncrease, 0, 100);
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;
        if (target.HasModifier<TokenProtection>())
        {
            @event.Cancel();
            if (source.AmOwner)
            {
                source.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown) / 2f);
                Coroutines.Start(MiscUtils.CoFlash(ChaosTokensPlugin.MainColor));
                Utils.Notification("Skill issue", true);
            }
        }
    }
}