using System;
using System.Collections;
using System.Linq;
using AmongUs.GameOptions;
using ChaosTokens.Modifiers;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs;
using TownOfUs.Events;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game.Alliance;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Modules;
using TownOfUs.Modules.Anims;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ChaosTokens;

public enum RpcCalls : uint
{
    Roll,
    IncreaseTokens,
    DecreaseTokens,
    
    TokenPosSwap,
    TokenRoleSwap,
    TokenRevive,
    TokenBlackmail,
}

public static class ChaosTokensRpc
{
    [MethodRpc((uint)RpcCalls.Roll)]
    public static void RpcRoll(this PlayerControl player)
    {
        if (!AmongUsClient.Instance.AmHost) return; // Host runs the checks

        if (!player.TryGetModifier<ChaosTokenModifier>(out var chaosToken)) return;
        if (chaosToken.Tokens <= 0)
        {
            return;
        }
        
        var playerRole = player.Data.Role;
        
        bool reroll = true;
        void Reroll()
        {
            reroll = true;
        }
        
        do
        {
            reroll = false;
            int random = Random.RandomRangeInt(0, Enum.GetValues<ChaosEffects>().Length);
            Logger<ChaosTokensPlugin>.Warning($"Rolled effect for {player.Data.PlayerName} - {(ChaosEffects)random}");
            switch ((ChaosEffects)random)
            {
                case ChaosEffects.Defense:
                    if (player.HasModifier<TokenProtection>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenProtection>((float)Random.RandomRange(15, 60));
                    break;
                case ChaosEffects.Speed:
                    if (player.HasModifier<TokenSpeed>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenSpeed>(Random.RandomRange(1.25f, 3f));
                    break;
                case ChaosEffects.Votes:
                    if (player.HasModifier<TokenVotes>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenVotes>();
                    break;
                case ChaosEffects.MoreTokens:
                    player.RpcIncreaseTokens(Random.RandomRangeInt(1, 3), true);
                    break;
                case ChaosEffects.Transparent:
                    if (player.HasModifier<TokenTransparent>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenTransparent>(Random.RandomRange(0f, 0.8f));
                    break;
                case ChaosEffects.LowerCooldown:
                    if (player.HasModifier<TokenCooldown>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenCooldown>(Random.RandomRange(0.5f, 1f));
                    break;
                case ChaosEffects.MediumIsReal:
                    if (player.HasModifier<TokenMedium>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenMedium>();
                    break;
                
                
                case ChaosEffects.RevealSelf:
                    if (player.HasModifier<TokenReveal>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenReveal>(player.Data.Role.Role, (byte)255, false);
                    break;
                case ChaosEffects.Death:
                    if (player.HasModifier<TokenDeath>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenDeath>();
                    break;
                case ChaosEffects.Blackmailed:
                    if (player.HasModifier<BlackmailedModifier>())
                    {
                        Reroll();
                        break;
                    }
                    RpcTokenBlackmail(player);
                    break;
                case ChaosEffects.Drunk:
                    if (player.HasModifier<TokenDrunk>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenDrunk>();
                    break;
                case ChaosEffects.FakeRevealSelf:
                    // The player will reveal as ANY role
                    // disabled, vanilla, ghost roles included
                    // why? because it's funny
                    if (player.HasModifier<TokenReveal>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenReveal>(RoleManager.Instance.AllRoles.Random().Role, (byte)255, true);
                    break;
                case ChaosEffects.Hyperactive:
                    if (player.HasModifier<TokenHyperactive>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenHyperactive>();
                    break;

                
                case ChaosEffects.RevealRandom:
                    var revealVictims = Helpers.GetAlivePlayers()
                        .Where(x => x.PlayerId != player.PlayerId)
                        .Where(x => !x.HasModifier<TokenReveal>())
                        .ToList();

                    if (revealVictims.Count <= 0)
                    {
                        Reroll();
                        break;
                    }

                    var revealVictim = revealVictims.Random();
                    revealVictim.RpcAddModifier<TokenReveal>(revealVictim.Data.Role.Role, player.PlayerId,
                        false);
                    break;
                case ChaosEffects.PositionSwap:
                    var swapVictim = Helpers.GetAlivePlayers()
                        .Where(x => !x.Data.IsDead)
                        .Where(x => x.PlayerId != player.PlayerId)
                        .Random();
                    RpcTokenPositionSwap(player, swapVictim);
                    break;
                case ChaosEffects.RoleSwap:
                    // This will be annoying as hell but the players have to be on the same team so it's kind of balanced
                    ModdedRoleTeams playerTeam;
                    if (playerRole is ICustomRole customRole)
                    {
                        playerTeam = customRole.Team;
                    }
                    else
                    {
                        playerTeam = (ModdedRoleTeams)playerRole.TeamType;
                    }

                    var roleSwapVictims = Helpers.GetAlivePlayers().Where(potentialVictim =>
                    {
                        if (potentialVictim.PlayerId == player.PlayerId) return false;

                        var potentialVictimRole = potentialVictim.Data.Role;
                        if (potentialVictimRole is ICustomRole customVictimRole)
                        {
                            return customVictimRole.Team == playerTeam;
                        }

                        if (playerTeam == ModdedRoleTeams.Custom)
                            return false;

                        return potentialVictimRole.TeamType == (RoleTeamTypes)playerTeam;
                    }).ToList();

                    if (roleSwapVictims.Count <= 0)
                    {
                        Reroll();
                        break;
                    }

                    RpcTokenRoleSwap(player, roleSwapVictims.Random());
                    break;
                case ChaosEffects.Revive:
                    var canBeRevived = PlayerControl.AllPlayerControls
                        .ToArray()
                        .Where(x => x.Data.IsDead && !x.Data.Disconnected)
                        .Where(x => x.GetModifier<DeathHandlerModifier>()?.RoundOfDeath == DeathEventHandlers.CurrentRound)
                        .ToList();

                    if (canBeRevived.Count <= 0)
                    {
                        Reroll();
                        break;
                    }

                    RpcTokenRevive(player, canBeRevived.Random());
                    break;
                case ChaosEffects.RandomModifier:
                    if (player.HasModifier<TokenRandomModifier>())
                    {
                        Reroll();
                        break;
                    }
                    player.RpcAddModifier<TokenRandomModifier>();
                    break;
            }
        } while (reroll);

        RpcDecreaseTokens(player, 1);
    }
    
    [MethodRpc((uint)RpcCalls.IncreaseTokens)]
    public static void RpcIncreaseTokens(this PlayerControl player, int amount, bool showNotification = false)
    {
        if (!player.TryGetModifier<ChaosTokenModifier>(out var chaosTokenModifier))
        {
            Logger<ChaosTokensPlugin>.Error($"Cannot increase tokens: player {player.Data.PlayerName} has no modifier, adding one");
            chaosTokenModifier = player.AddModifier<ChaosTokenModifier>(amount, showNotification);
        }
        else
        {
            chaosTokenModifier.IncreaseTokens(amount, showNotification);
        }
    }

    [MethodRpc((uint)RpcCalls.DecreaseTokens)]
    public static void RpcDecreaseTokens(this PlayerControl player, int amount)
    {
        if (!player.TryGetModifier<ChaosTokenModifier>(out var chaosTokenModifier))
        {
            Logger<ChaosTokensPlugin>.Error($"Cannot decrease tokens: player {player.Data.PlayerName} has no modifier");
            return;
        }
        
        chaosTokenModifier.DecreaseTokens(amount);
    }
    
    [MethodRpc((uint)RpcCalls.TokenPosSwap)]
    public static void RpcTokenPositionSwap(PlayerControl player, PlayerControl victim)
    {
        Vector3 pos1 = player.GetTruePosition();
        Vector3 pos2 = victim.GetTruePosition();
               
        TransporterRole.Transport(player, pos2);
        TransporterRole.Transport(victim, pos1);

        if (player.AmOwner)
        {
            Utils.Notification("<b>You swapped positions with someone!</b>");
        }
        
        if (victim.AmOwner)
        {
            Utils.Notification("<b>Someone swapped positions with you!</b>");
        }
    }
    
    [MethodRpc((uint)RpcCalls.TokenRoleSwap)]
    public static void RpcTokenRoleSwap(PlayerControl player, PlayerControl victim)
    {
        if (player.AmOwner)
        {
            Utils.Notification("<b>You swapped roles with someone!</b>");
        }
        if (victim.AmOwner)
        {
            Utils.Notification("<b>Someone swapped roles with you!</b>");
        }
        
        RoleTypes role1 = player.Data.Role.Role;
        RoleTypes role2 = victim.Data.Role.Role;

        player.ChangeRole((ushort)role2);
        victim.ChangeRole((ushort)role1);
    }

    [MethodRpc((uint) RpcCalls.TokenRevive)]
    public static void RpcTokenRevive(PlayerControl source, PlayerControl target)
    {
        Coroutines.Start(CoRevivePlayer(target));

        if (target.AmOwner)
        {
            Utils.Notification("<b>Someone revived you, you are back from the dead!</b>");
            Coroutines.Start(MiscUtils.CoFlash(ChaosTokensPlugin.MainColor));
        }

        if (source.AmOwner)
        {
            Utils.Notification("<b>You revived a random person!</b>");
        }
    }
    
    [MethodRpc((uint)RpcCalls.TokenBlackmail)]
    public static void RpcTokenBlackmail(PlayerControl player)
    {
        if (player.AmOwner)
        {
            Utils.Notification("<b>You are blackmailed!</b>");
        }

        player.AddModifier<BlackmailedModifier>(player.PlayerId);
    }
    
    [HideFromIl2Cpp]
    public static IEnumerator CoRevivePlayer(PlayerControl dead)
    {
        var roleWhenAlive = dead.GetRoleWhenAlive();

        var body = GameObject.FindObjectsOfType<DeadBody>()
            .FirstOrDefault(b => b.ParentId == dead.PlayerId);
        var position = ShipStatus.Instance.AllVents.Random().transform.position;

        if (body != null)
        {
            GameObject.Destroy(body.gameObject);
        }

        // Notify the killer about revival to make it a bit more fair
        var killerId = GameHistory.KilledPlayers.FirstOrDefault(x => x.VictimId == dead.PlayerId)?.KillerId;
        if (killerId.HasValue)
        {
            var killer = MiscUtils.PlayerById(killerId.Value);
            if (killer != null && killer.AmOwner)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Altruist));
            }
        }

        yield return new WaitForSeconds(1);

        if (!MeetingHud.Instance)
        {
            GameHistory.ClearMurder(dead);

            dead.Revive();

            dead.transform.position = new Vector2(position.x, position.y);
            if (dead.AmOwner)
            {
                PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(new Vector2(position.x, position.y));
            }

            if (ModCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == dead.PlayerId)
            {
                ModCompatibility.ChangeFloor(dead.transform.position.y > -7);
            }

            if (dead.AmOwner && !dead.HasModifier<LoverModifier>())
            {
                HudManager.Instance.Chat.gameObject.SetActive(false);
            }

            // return player from ghost role back to what they were when alive
            dead.ChangeRole((ushort)roleWhenAlive!.Role, false);

            if (dead.Data.Role is IAnimated animated)
            {
                animated.IsVisible = true;
                animated.SetVisible();
            }

            foreach (var button in CustomButtonManager.Buttons.Where(x => x.Enabled(dead.Data.Role))
                         .OfType<IAnimated>())
            {
                button.IsVisible = true;
                button.SetVisible();
            }

            foreach (var modifier in dead.GetModifiers<GameModifier>().Where(x => x is IAnimated))
            {
                if (modifier is IAnimated animatedMod)
                {
                    animatedMod.IsVisible = true;
                    animatedMod.SetVisible();
                }
            }

            dead.RemainingEmergencies = 0;
        }

        yield return null;
    }
}

