using System.Linq;
using HarmonyLib;
using MiraAPI.Modifiers;

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
}