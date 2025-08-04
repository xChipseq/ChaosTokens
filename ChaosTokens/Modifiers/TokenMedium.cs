using System.Linq;
using HarmonyLib;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public sealed class TokenMedium : BaseModifier
{
    public override string ModifierName => "Token Medium";
    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        if (!Player.AmOwner) return;

        Utils.Notification("<b>You can now see ghosts, Spoooky!</b>");
    }

    public override void OnDeactivate()
    {
        if (Player.AmOwner)
        {
            PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x.Data.IsDead)
                .Do(x => x.Visible = false);
        }
    }

    public override void FixedUpdate()
    {
        if (Player.AmOwner)
        {
            PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x.Data.IsDead)
                .Do(x => x.Visible = true);
        }
    }

    public override void OnMeetingStart()
    {
        Player.RemoveModifier(this);
    }
}