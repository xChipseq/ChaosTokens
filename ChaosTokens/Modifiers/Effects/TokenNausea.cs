using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ChaosTokens.Modifiers.Effects;

public sealed class TokenNausea : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Nausea;
    public override string ModifierName => "Token Nausea";
    public override string Notification => "Your vision is distorted.";
    public override bool Negative => true;

    public static float[][] valueSets = [
        [
            0.007f,
            6f,
            50f,
        ],
        [
            0.032f,
            3f,
            7f,
        ],
        [
            0.04f,
            3f,
            2f,
        ],
    ];

    private SpriteRenderer _filter;

    public override void OnActivate()
    {
        base.OnActivate();
        _filter = Utils.CreatePostprocessFilter(Assets.NauseaMaterial.LoadAsset());

        var valueSet = valueSets.Random();
        _filter.material.SetFloat("_Magnitude", valueSet[0]);
        _filter.material.SetFloat("_Speed", valueSet[1]);
        _filter.material.SetFloat("_Frequency", valueSet[2]);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        _filter.gameObject.DestroyImmediate();
        _filter = null;
    }
}