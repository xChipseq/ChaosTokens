using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using TownOfUs.Utilities.Appearances;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public sealed class TokenSpeed(float speed) : BaseModifier
{
    public override string ModifierName => "Token Speed";
    public override bool HideOnUi => true;

    public float Speed { get; private set; } = speed;
    
    public override void OnActivate()
    {
        if (!Player.AmOwner) return;
        Utils.Notification("<b>You are now faster!</b>");
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }
}