using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public sealed class TokenHyperactive : BaseModifier
{
    public override string ModifierName => "Token Hyperactive";
    public override bool HideOnUi => true;
    
    public bool EffectActive = false;

    public override void OnActivate()
    {
        if (!Player.AmOwner) return;
        
        Utils.Notification("<b>You now have ADHD.</b>", true);
    }

    public override void FixedUpdate()
    {
        int random = Random.RandomRangeInt(1, EffectActive ? 4 : 50);

        if (random == 1)
        {
            EffectActive = !EffectActive;
        }
        
        random = Random.RandomRangeInt(1, 100);
        if (random == 1)
        {
            foreach (var ability in CustomButtonManager.Buttons)
            {
                if (!ability.Button.isActiveAndEnabled) return;
                if (!ability.CanClick()) continue;
                if (ability.EffectActive) continue;

                ability.ClickHandler();
            }
        }
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }
}