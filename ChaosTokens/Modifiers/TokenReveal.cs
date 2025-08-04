using AmongUs.GameOptions;
using MiraAPI.Utilities;
using TownOfUs.Modifiers;
using TownOfUs.Roles.Crewmate;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public sealed class TokenReveal(RoleTypes role, byte revealSource, bool fakeReveal = false) : RevealModifier((int)ChangeRoleResult.UpdateInfo, true, RoleManager.Instance.GetRole(role))
{
    public override string ModifierName => "Token Reveal";
    
    public byte? RevealSource => revealSource;
    public bool FakeReveal => fakeReveal;

    public override void OnActivate()
    {
        base.OnActivate();
        if (revealSource != 255)
        {
            if (PlayerControl.LocalPlayer.PlayerId == revealSource)
            {
                Utils.Notification("<b>You revealed the role of a random person!</b>");
            }
        }

        if (revealSource != 255 && fakeReveal && Player.AmOwner)
        {
            Utils.Notification("<b>You revealed to everyone as a random fake role, good luck!</b>", true);
            return;
        }

        if (Player.AmOwner)
        {
            Utils.Notification("<b>Your role has been revealed to everyone!</b>", true);
        }
    }
}