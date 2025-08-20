using MiraAPI.Hud;
using UnityEngine;

namespace ChaosTokens.Modifiers.Effects;

public sealed class TokenHyperactive : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Hyperactive;
    public override string ModifierName => "Token Hyperactive";
    public override string Notification => "You now have ADHD.";
    public override bool Negative => true;
    
    public bool EffectActive = false;

    public override void FixedUpdate()
    {
        if (MeetingHud.Instance)
        {
            return;
        }
        
        int random = Random.RandomRangeInt(1, EffectActive ? 4 : 50);

        if (random == 1)
        {
            EffectActive = !EffectActive;
        }

        if (Player.AmOwner)
        {
            foreach (var ability in CustomButtonManager.Buttons)
            {
                random = Random.RandomRangeInt(1, 100);
                if (random == 1)
                {
                    if (!ability.Button?.isActiveAndEnabled ?? false) continue;
                    if (ability.Location == ButtonLocation.BottomLeft) continue; // not the best solution, but stops modifier buttons from being pressed
                    if (!ability.CanClick()) continue;
                    if (ability.EffectActive) continue;

                    ability.ClickHandler();
                }
            }
        }
    }
}