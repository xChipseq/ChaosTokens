using AmongUs.GameOptions;
using TownOfUs.Modifiers;
using TownOfUs.Utilities;

namespace ChaosTokens.Modifiers;

public sealed class RoleReveal(RoleTypes role, byte revealSource) : RevealModifier((int)ChangeRoleResult.Nothing, true, RoleManager.Instance.GetRole(role))
{
    public override string ModifierName => "Role Reveal";

    public PlayerControl RevealSource;

    public override void OnActivate()
    {
        base.OnActivate();
        
        RevealSource = MiscUtils.PlayerById(revealSource);
    }
}