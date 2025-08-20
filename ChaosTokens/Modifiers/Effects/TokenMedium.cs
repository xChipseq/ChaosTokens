using System.Collections;
using System.Linq;
using HarmonyLib;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs;
using TownOfUs.Utilities;
using UnityEngine;

namespace ChaosTokens.Modifiers.Effects;

public sealed class TokenMedium : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.MediumIsReal;
    public override string ModifierName => "Token Medium";
    public override string Notification => "You can now see ghosts, spoooky!";
    public override bool Negative => false;

    public override void OnActivate()
    {
        base.OnActivate();
        
        if (Player.AmOwner)
        {
            PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x.Data.IsDead && !x.Data.Disconnected)
                .Where(x => !x.HasModifier<VisibleGhost>())
                .Do(x => x.AddModifier<VisibleGhost>());
        }

        if (PlayerControl.LocalPlayer.HasDied())
        {
            Coroutines.Start(MiscUtils.CoFlash(ChaosTokensPlugin.MainColor, 2));
            Coroutines.Start(CoArrow(Player));
        }
    }

    public override void OnDeactivate()
    {
        if (Player.AmOwner)
        {
            PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x.Data.IsDead && !x.Data.Disconnected)
                .Where(x => x.HasModifier<VisibleGhost>())
                .Do(x => x.RemoveModifier<VisibleGhost>());
        }
    }

    private static IEnumerator CoArrow(PlayerControl player)
    {
        var arrow = MiscUtils.CreateArrow(player.transform, TownOfUsColors.Medium);
        for (float time = 0; time <= 2; time += Time.deltaTime)
        {
            arrow.target = player.GetTruePosition();
            yield return new WaitForEndOfFrame();
        }
        arrow.gameObject.Destroy();
    }
}