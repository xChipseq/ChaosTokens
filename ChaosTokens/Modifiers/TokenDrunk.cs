using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public sealed class TokenDrunk : BaseModifier
{
    public override string ModifierName => "Token Drunk";
    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        if (!Player.AmOwner) return;
        
        Utils.Notification("<b>Your controls are inverted o_0</b>", true);
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }
}