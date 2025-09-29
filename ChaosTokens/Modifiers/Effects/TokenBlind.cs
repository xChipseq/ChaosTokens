using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using Reactor.Utilities;
using UnityEngine;

namespace ChaosTokens.Modifiers.Effects;

public class TokenBlind : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Blind;
    public override string ModifierName => "Token Blind";
    public override string Notification => "You are blind. Partially";
    public override bool Negative => true;
    public override bool RemoveOnDeath => true;

    private Color ogColor;
    
    public override void OnActivate()
    {
        base.OnActivate();
        var quad = HudManager.Instance.ShadowQuad;
        ogColor = quad.material.GetColor(ShaderID.Color);
        quad.material.SetColor(ShaderID.Color, Color.black);
        
        // Skeld
        GameObject.Find("Hull2")?.SetActive(false);
        GameObject.Find("HullItems")?.SetActive(false);
        // Polus
        GameObject.Find("Background")?.SetActive(false);
        // Airship
        GameObject.Find("engine_pipewheel")?.SetActive(false);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        var quad = HudManager.Instance.ShadowQuad;
        quad.material.SetColor(ShaderID.Color, ogColor);
    }
}