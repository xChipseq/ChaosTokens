using ChaosTokens.Modifiers;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfUs.Buttons;
using UnityEngine;

namespace ChaosTokens.Buttons;

public class CheatButton : TownOfUsButton
{
    public override string Name => "Cheat";
    public override float Cooldown => 1;
    public override float InitialCooldown => 1;
    public override LoadableAsset<Sprite> Sprite => MiraAssets.Cog;
    public override Color TextOutlineColor => Color.green;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;
    public override string Keybind => Keybinds.ModifierAction;

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcIncreaseTokens(1, true);
    }

    public override bool Enabled(RoleBehaviour role)
    {
        return ChaosTokensPlugin.DevBuild && !LobbyBehaviour.Instance;
    }
}