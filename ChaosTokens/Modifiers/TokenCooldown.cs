using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Options.Roles.Crewmate;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public sealed class TokenCooldown(float multiplier) : BaseModifier
{
    public override string ModifierName => "Token Cooldown";
    public override bool HideOnUi => true;

    public float Multiplier { get; } = multiplier;
    
    public override void OnActivate()
    {
        if (!Player.AmOwner) return;
            
        Utils.Notification($"<b>Your cooldown decreases faster!</b>");
    }

    public override void FixedUpdate()
    {
        if (!Player.AmOwner) return;

        foreach (var ability in CustomButtonManager.Buttons)
        {
            if (ability.EffectActive) continue;
            if (ability.TimerPaused) continue;

            ability.DecreaseTimer(Time.deltaTime * Multiplier);
        }

        Player.killTimer -= Time.deltaTime * Multiplier;
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }
}