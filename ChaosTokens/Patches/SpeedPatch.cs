using ChaosTokens.Modifiers.Effects;
using HarmonyLib;
using MiraAPI.Modifiers;
using TownOfUs.Utilities.Appearances;

namespace ChaosTokens.Patches;

[HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetPlayerSpeedMod))]
public static class PlayerSpeedPatch
{
    public static void Postfix(PlayerControl pc, ref float __result)
    {
        if (pc.TryGetModifier<TokenSpeed>(out var tokenSpeed))
        {
            __result *= tokenSpeed.Speed;
        }
        
        if (pc.TryGetModifier<TokenHyperactive>(out var tokenHyperactive))
        {
            if (tokenHyperactive.EffectActive)
            {
                __result *= 5;
            }
        }
        
        if (pc.TryGetModifier<TokenDrunk>(out var tokenDrunk))
        {
            __result *= -1;
        }
    }
}