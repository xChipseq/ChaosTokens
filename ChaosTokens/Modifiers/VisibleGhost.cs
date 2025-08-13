using MiraAPI.Modifiers;

namespace ChaosTokens.Modifiers;

public sealed class VisibleGhost : BaseModifier
{
    public override string ModifierName => "Visible Ghost";
    public override bool HideOnUi => true;

    public override void FixedUpdate()
    {
        if (!Player.Data.IsDead)
        {
            ModifierComponent?.RemoveModifier(this);
            return;
        }
        
        Player.Visible = true;
    }
}