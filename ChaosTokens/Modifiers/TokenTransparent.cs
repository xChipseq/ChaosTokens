using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using TownOfUs.Modifiers.Game.Universal;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public sealed class TokenTransparent(float visibility) : BaseModifier
{
    public override string ModifierName => "Token Transparent";
    public override bool HideOnUi => true;

    public float Visibility { get; } = visibility;

    public override void OnActivate()
    {
        if (!Player.AmOwner) return;
        
        Utils.Notification("<b>You are now transparent!</b>");
    }

    public override void FixedUpdate()
    {
        ShyModifier.SetVisibility(Player, Visibility, true);
    }

    public override void OnDeactivate()
    {
        ShyModifier.SetVisibility(Player, 1);
    }
}