using MiraAPI.Hud;
using UnityEngine;

namespace ChaosTokens.Modifiers.Effects;

public sealed class TokenCooldown(float multiplier) : TokenEffect
{
    public override string ModifierName => "Token Cooldown";
    public override ChaosEffects Effect => ChaosEffects.LowerCooldown;
    public override string Notification => "Your cooldown decreases faster!";
    public override bool Negative => false;

    public float Multiplier { get; } = multiplier;
    
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
}