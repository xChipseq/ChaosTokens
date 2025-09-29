using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using UnityEngine;

namespace ChaosTokens.Modifiers.Effects;

public class TokenScreenFlip : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.ScreenFlip;
    public override string ModifierName => "Token Screen Flip";
    public override string Notification => "Your screen is now flipped.";
    public override bool Negative => true;
    public override bool RemoveOnDeath => true;

    public override void OnActivate()
    {
        base.OnActivate();
        Coroutines.Start(CoFlipCamera(-1));
    }
    
    public override void OnDeactivate()
    {
        base.OnDeactivate();
        Coroutines.Start(CoFlipCamera(1));
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }

    public IEnumerator CoFlipCamera(float value)
    {
        var cam = Camera.main!;
        var quad = HudManager.Instance.ShadowQuad;
        var camStart = cam.transform.localScale;
        var camGoal = cam.transform.localScale with { y = value };
        var quadStart = quad.transform.localScale;
        var quadGoal = quad.transform.localScale with { y = -quad.transform.localScale.y };
        
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            cam.transform.localScale = Vector3.Lerp(camStart, camGoal, Math.Clamp(t, 0, 1));
            quad.transform.localScale = Vector3.Lerp(quadStart, quadGoal, Math.Clamp(t, 0, 1));
            yield return new WaitForEndOfFrame();
        }

        var buttons = new List<ActionButton>();
        var bp = HudManager.Instance.transform.FindChild("Buttons");
        buttons.AddRange(bp.FindChild("BottomRight").GetComponentsInChildren<ActionButton>());
        buttons.AddRange(bp.FindChild("BottomLeft").GetComponentsInChildren<ActionButton>());
        foreach (var button in buttons)
        {
            button.transform.localPosition = button.transform.localPosition with { y = button.transform.localPosition.y * value };
        }
    }
}