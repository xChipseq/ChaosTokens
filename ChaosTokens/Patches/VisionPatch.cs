using ChaosTokens.Modifiers.Effects;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfUs.Modifiers.Game.Crewmate;
using TownOfUs.Modifiers.Impostor;
using TownOfUs.Options;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace ChaosTokens.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
public static class VisionPatch
{
    public static void Postfix(ShipStatus __instance, NetworkedPlayerInfo player, ref float __result)
    {
        if (player != null)
        {
            var pc = MiscUtils.PlayerById(player.PlayerId);
            if (pc.TryGetModifier<TokenVision>(out var mod))
            {
                __result *= mod.Multiplier;
            }
        }
    }
}