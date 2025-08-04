using MiraAPI.Utilities;
using TownOfUs.Modifiers;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public class TokenProtection(float duration) : BaseShieldModifier
{
    public override string ModifierName => "Token Protection";
    public override bool HideOnUi => true;
    
    public override void OnActivate()
    {
        StartTimer();
        TimeRemaining = duration;

        if (!Player.AmOwner) return;
        Utils.Notification($"<b>You received protection for {duration} seconds!</b>");
    }
}