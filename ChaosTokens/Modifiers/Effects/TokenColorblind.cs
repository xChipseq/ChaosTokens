using System.Collections;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace ChaosTokens.Modifiers.Effects;

public sealed class TokenColorblind : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Colorblind;
    public override string ModifierName => "Token Colorblind";
    public override string Notification => "You are now colorblind.";
    public override bool Negative => true;

    private static readonly int _threshold = Shader.PropertyToID("_Threshold");
    private SpriteRenderer _filter;

    public override void OnActivate()
    {
        base.OnActivate();
        _filter = Utils.CreatePostprocessFilter(Assets.ColorblindMaterial.LoadAsset());
        Coroutines.Start(CoAnimateFilter(_filter));
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        _filter.gameObject.DestroyImmediate();
        _filter = null;
    }

    private IEnumerator CoAnimateFilter(SpriteRenderer rend)
    {
        rend.material.SetFloat(_threshold, 0);

        float value = 0;
        while (value < 1)
        {
            if (rend == null)
            {
                yield return null;
            }
            rend.material.SetFloat(_threshold, value + Time.deltaTime);
            value = rend.material.GetFloat(_threshold);
            yield return new WaitForEndOfFrame();
        }
    }
}