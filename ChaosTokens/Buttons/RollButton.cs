using ChaosTokens.Modifiers;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using UnityEngine;

namespace ChaosTokens.Buttons;

public class RollButton : TownOfUsButton
{
    public override string Name => "Roll";
    public override float Cooldown => OptionGroupSingleton<ChaosTokensOptions>.Instance.RollCooldown;
    public override float InitialCooldown => OptionGroupSingleton<ChaosTokensOptions>.Instance.InitialRollCooldown;
    public override LoadableAsset<Sprite> Sprite => ChaosTokensPlugin.DiceButton;
    public override Color TextOutlineColor => ChaosTokensPlugin.MainColor;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;
    public override string Keybind => Keybinds.ModifierAction;

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcRoll();
    }

    public override bool Enabled(RoleBehaviour role)
    {
        return PlayerControl.LocalPlayer != null &&
               PlayerControl.LocalPlayer.HasModifier<ChaosTokenModifier>() &&
               !PlayerControl.LocalPlayer.Data.IsDead;
    }
}