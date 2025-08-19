using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using ChaosTokens.Modifiers.Effects;
using ChaosTokens.Options;
using Cpp2IL.Core.Extensions;
using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Voting;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Utilities;
using UnityEngine;

namespace ChaosTokens;

public static class TokenEvents
{
    private static float DoubleTokenChance { get; set; } = OptionGroupSingleton<TokenHandingOptions>.Instance.InitialDoubleTokenChance;
    private static readonly Dictionary<byte, float> PriorityTable = new();
    
    [RegisterEvent]
    public static void HandleVotesEventHandler(HandleVoteEvent @event)
    {
        if (@event.TargetId == 253 && ModifierUtils.GetPlayersWithModifier<TokenNoSkip>().Any())
        {
            @event.Cancel();
        }
        
        if (@event.VoteData.Owner.HasModifier<TokenVotes>())
        {
            @event.VoteData.SetRemainingVotes(0);

            int votes = 1;
            float chance = 100;

            while (chance > 0)
            {
                if (Random.RandomRange(1, 100) < chance)
                {
                    votes++;
                    chance -= Random.RandomRange(15, 35);
                }
                else
                {
                    break;
                }
            }

            for (var i = 0; i < votes; i++)
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
            
            // ToUM adds time when someone dies during a meeting, because of that the proceed anim is longer
            // We add the time it subtracted to fix this
            var timer = (int)OptionGroupSingleton<TownOfUs.Options.GeneralOptions>.Instance.AddedMeetingDeathTimer;
            MeetingHud.Instance.discussionTimer += timer;
        });

        try
        {
            MeetingHud.Instance.playerStates.Do(x => x.transform.FindChild("TokenDeathIcon").gameObject.DestroyImmediate());
            MeetingHud.Instance.playerStates.Do(x => x.transform.FindChild("TokenDeathIcon").gameObject.DestroyImmediate());
        }
        catch {} // don't care
    }

    [RegisterEvent]
    public static void ProcessVotesEventHandler(ProcessVotesEvent @event)
    {
        if (@event.ExiledPlayer == null) return;

        var player = MiscUtils.PlayerById(@event.ExiledPlayer.PlayerId);
        if (player.HasModifier<TokenDeath>())
        {
            @event.ExiledPlayer = null;
        }
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        ModifierUtils.GetActiveModifiers<TokenEffect>().Do(m =>
        {
            if (m.RemoveAfterMeeting)
            {
                m.Player.RemoveModifier(m);
            }
        });
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!OptionGroupSingleton<GeneralOptions>.Instance.EnableChaosTokens)
        {
            return;
        }
        if (!AmongUsClient.Instance.AmHost)
        {
            return;
        }
        if (@event.TriggeredByIntro)
        {
            PriorityTable.Clear();
            PlayerControl.AllPlayerControls
                .ToArray()
                .Do(p => PriorityTable.Add(p.PlayerId, 10));
            
            if (!OptionGroupSingleton<TokenHandingOptions>.Instance.TokensEnabledFirstRound)
            {
                return;
            }
        }

        int min = (int)OptionGroupSingleton<TokenHandingOptions>.Instance.TokensMin;
        int max = (int)OptionGroupSingleton<TokenHandingOptions>.Instance.TokensMax;

        if (max == 0)
        {
            max = int.MaxValue;
        }

        if (min > max)
        {
            min = max;
        }

        var potentialPlayers = Helpers.GetAlivePlayers();
        potentialPlayers.Do(p =>
        {
            // Just to be sure
            if (!PriorityTable.ContainsKey(p.PlayerId))
            {
                PriorityTable.Add(p.PlayerId, 10);
            }
        });
        potentialPlayers.Shuffle();
        potentialPlayers = potentialPlayers.OrderBy(x => PriorityTable[x.PlayerId]).ToList();
        
        int tokensToHandle = Mathf.Clamp(Random.RandomRangeInt(min, max), 0, potentialPlayers.Count);
        foreach (var player in potentialPlayers.Clone())
        {
            // Reset when player gets a token
            PriorityTable[player.PlayerId] = 10;
            
            int tokens = 1;
            if (Random.RandomRangeInt(1, 100) < DoubleTokenChance && tokensToHandle > 1)
            {
                tokens = 2;
                PriorityTable[player.PlayerId] = 11; // double token decreases the priority
            }

            player.RpcIncreaseTokens(tokens, true);
            
            tokensToHandle -= tokens;
            potentialPlayers.Remove(player);
            if (tokensToHandle <= 0)
            {
                break;
            }
        }
        
        // Players without a token get priority for the next handing
        potentialPlayers.Do(p => PriorityTable[p.PlayerId]--);

        // Simplest solution to ignore handing priority: reset everything if disabled
        if (!OptionGroupSingleton<TokenHandingOptions>.Instance.TokenHandingPriority)
        {
            potentialPlayers.Do(p => PriorityTable[p.PlayerId] = 10);
        }
        
        DoubleTokenChance = Mathf.Clamp(DoubleTokenChance + OptionGroupSingleton<TokenHandingOptions>.Instance.DoubleTokenIncrease, 0, 100);
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;
        if (target.HasModifier<TokenDefense>())
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