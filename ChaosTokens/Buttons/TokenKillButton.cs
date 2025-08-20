using ChaosTokens.Modifiers.Effects;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfUs.Buttons;
using TownOfUs.Options.Modifiers.Alliance;
using TownOfUs.Utilities;
using UnityEngine;

namespace ChaosTokens.Buttons;

public class TokenKillButton : TownOfUsTargetButton<PlayerControl>
{
    public override string Name => "Kill";
    public override float Cooldown => 10;
    public override LoadableAsset<Sprite> Sprite => Assets.DiceButton;
    public override Color TextOutlineColor => ChaosTokensPlugin.MainColor;
    public override ButtonLocation Location => ButtonLocation.BottomLeft;

    protected override void OnClick()
    {
        PlayerControl.LocalPlayer.RpcCustomMurder(Target);
        PlayerControl.LocalPlayer.RpcRemoveModifier<TokenOneTimeKill>();
    }
    
    public override PlayerControl GetTarget()
    {
        if (!OptionGroupSingleton<LoversOptions>.Instance.LoversKillEachOther && PlayerControl.LocalPlayer.IsLover())
        {
            return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false, x => !x.IsLover());
        }
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    public override void SetOutline(bool active)
    {
        if (Target != null && !PlayerControl.LocalPlayer.HasDied())
        {
            Target.cosmetics.currentBodySprite.BodySprite.SetOutline(active ? Palette.ImpostorRed : null);
        }
    }

    public override bool Enabled(RoleBehaviour role)
    {
        return PlayerControl.LocalPlayer != null &&
               PlayerControl.LocalPlayer.HasModifier<TokenOneTimeKill>() &&
               !PlayerControl.LocalPlayer.Data.IsDead;
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        if (!Button) return;
        Button.graphic.sprite = HudManager.Instance?.KillButton?.graphic?.sprite ?? MiraAssets.Empty.LoadAsset();
        Button.graphic.color = ChaosTokensPlugin.MainColor;
    }
}