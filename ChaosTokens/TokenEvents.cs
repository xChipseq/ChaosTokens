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
    private static readonly Dictionary<byte, float> WeightTable = new();

    [RegisterEvent]
    public static void HandleVotesEventHandler(HandleVoteEvent @event)
    {
        if (@event.TargetId == 253 && ModifierUtils.GetPlayersWithModifier<TokenNoSkip>().Any())
        {
            @event.Cancel();
            return;
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
            WeightTable.Clear();
            PlayerControl.AllPlayerControls
                .ToArray()
                .Do(p => WeightTable.Add(p.PlayerId, 10));

            if (!OptionGroupSingleton<TokenHandingOptions>.Instance.TokensEnabledFirstRound)
            {
                return;
            }
        }

        var min = (int)OptionGroupSingleton<TokenHandingOptions>.Instance.TokensMin;
        var max = (int)OptionGroupSingleton<TokenHandingOptions>.Instance.TokensMax;
        if (max == 0)
        {
            max = int.MaxValue;
        }
        if (min > max)
        {
            min = max;
        }

        var potentialPlayers = Helpers.GetAlivePlayers();
        min = Mathf.Min(min, potentialPlayers.Count);
        max = Mathf.Min(max, potentialPlayers.Count);

        var tokensToHand = Mathf.Min(Random.RandomRangeInt(min, max + 1), potentialPlayers.Count);
        var winners = WeightedSample(potentialPlayers, WeightTable, tokensToHand, 3);
        foreach (var player in winners)
        {
            int tokens = Random.RandomRangeInt(1, 100) < DoubleTokenChance ? 2 : 1;
            player.RpcIncreaseTokens(tokens, true);
        }

        // i mean if we reset the weights table, everyone's chance is still the same?
        if (!OptionGroupSingleton<TokenHandingOptions>.Instance.WeightedTokens)
        {
            potentialPlayers.Do(p => WeightTable[p.PlayerId] = 10);
        }

        var doubleChance = DoubleTokenChance + OptionGroupSingleton<TokenHandingOptions>.Instance.DoubleTokenIncrease;
        DoubleTokenChance = Mathf.Clamp(doubleChance, 0, 100);
    }

    [RegisterEvent]
    public static void GameStartEventRegister(IntroBeginEvent @event)
    {
        ChaosTokensRpc.RevealsLeft = (int)OptionGroupSingleton<BalanceOptions>.Instance.MaxRoleReveals;
        ChaosTokensRpc.SwapsLeft = (int)OptionGroupSingleton<BalanceOptions>.Instance.MaxRoleSwaps;
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;
        if (target.HasModifier<TokenDefense>())
        {
            if (source.HasModifier<IndirectAttackerModifier>())
            {
                return;
            }

            if (source == target)
            {
                return;
            }

            @event.Cancel();
            if (source.AmOwner)
            {
                source.SetKillTimer(GameOptionsManager.Instance.CurrentGameOptions.GetFloat(FloatOptionNames.KillCooldown) / 2f);
                Coroutines.Start(MiscUtils.CoFlash(ChaosTokensPlugin.MainColor));
                Utils.Notification("Skill issue", true);
            }
        }
    }

    private static PlayerControl[] WeightedSample(List<PlayerControl> players, Dictionary<byte, float> weights, int n, float adjustment)
    {
        var pool = new List<PlayerControl>(players);
        var winners = new List<PlayerControl>();
        var rng = new System.Random();

        for (int i = 0; i < n; i++)
        {
            float total = pool.Sum(p => weights[p.PlayerId]);
            float roll = (float)rng.NextDouble() * total;
            float cumulative = 0;

            foreach (var player in pool)
            {
                cumulative += weights[player.PlayerId];
                if (roll < cumulative)
                {
                    winners.Add(player);
                    pool.Remove(player);
                    break;
                }
            }
        }

        foreach (var player in players)
        {
            if (winners.Contains(player))
            {
                weights[player.PlayerId] = Mathf.Max(1, weights[player.PlayerId] - adjustment);
            }
            else
            {
                weights[player.PlayerId] += adjustment;
            }
        }

        return winners.ToArray();
    }
}