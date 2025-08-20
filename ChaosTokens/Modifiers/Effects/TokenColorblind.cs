using System.Collections;
using HarmonyLib;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
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
        if (!Player.AmOwner) return;
        
        _filter = Utils.CreatePostprocessFilter(Assets.ColorblindMaterial.LoadAsset());
        Coroutines.Start(CoAnimateFilter(_filter));
        Helpers.GetAlivePlayers().Do(p =>
        {
            if (!p.AmOwner)
            {
                p.cosmetics.nameText.gameObject.SetActive(false);
                p.cosmetics.colorBlindText.gameObject.SetActive(false);
            }
        });
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (Player.AmOwner)
        {
            _filter.gameObject.DestroyImmediate();
            _filter = null;
            Helpers.GetAlivePlayers().Do(p =>
            {
                if (Player.GetAppearanceType() != TownOfUsAppearances.Swooper &&
                    Player.GetAppearanceType() != TownOfUsAppearances.Camouflage)
                {
                    p.cosmetics.nameText.gameObject.SetActive(true);
                    p.cosmetics.colorBlindText.gameObject.SetActive(true);
                }
            });
        }
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