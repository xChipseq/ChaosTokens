using System;
using Reactor.Utilities;
using TownOfUs.Modifiers.Game.Universal;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;

namespace ChaosTokens.Modifiers.Effects;

public sealed class TokenInvisible : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Invisible;
    public override string ModifierName => "Token Invisible";
    public override string Notification => "You have now become... invisible?";
    public override bool Negative => false;

    private const float InvisDelay = 1f;
    private const float FinalTransparency = 0f;

    private string _ogSkin;
    private bool _stop = false;
    private DateTime _lastMoved;
    
    public override void OnActivate()
    {
        base.OnActivate();
        _ogSkin = Player.cosmetics.skin.data.ProdId;
        Player.cosmetics.SetSkin("skin_Box1skin", Player.cosmetics.ColorId);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        ShyModifier.SetVisibility(Player, 1f);
        Player.cosmetics.SetSkin(_ogSkin, Player.cosmetics.ColorId);
    }
    
    public override void Update()
    {
        if (IntroCutscene.Instance)
        {
            return;
        }

        if (Player == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer == null)
        {
            return;
        }

        if (Player.HasDied())
        {
            if (!_stop)
            {
                _stop = true;
                ShyModifier.SetVisibility(Player, 1f);
            }
            return;
        }

        _stop = false;

        // check movement by animation
        var playerPhysics = Player.MyPhysics;
        var currentPhysicsAnim = playerPhysics.Animations.Animator.GetCurrentAnimation();
        if (currentPhysicsAnim != playerPhysics.Animations.group.IdleAnim)
        {
            _lastMoved = DateTime.UtcNow;
        }

        if (Player.GetAppearanceType() == TownOfUsAppearances.Swooper)
        {
            var opacity = 0f;

            if ((PlayerControl.LocalPlayer.IsImpostor() && Player.Data.Role is SwooperRole) ||
                (Player.AmOwner && Player.Data.Role is SwooperRole))
            {
                opacity = 0.1f;
            }

            ShyModifier.SetVisibility(Player, opacity, true);
        }
        else if (Player.GetAppearanceType() == TownOfUsAppearances.Camouflage)
        {
            ShyModifier.SetVisibility(Player, 1f, true);
        }
        else if (Player.GetAppearanceType() == TownOfUsAppearances.Morph || Player.GetAppearanceType() == TownOfUsAppearances.Mimic)
        {
            ShyModifier.SetVisibility(Player, 1f);
        }
        else
        {
            var timeSpan = DateTime.UtcNow - _lastMoved;

            if (timeSpan.TotalMilliseconds / 1000f < InvisDelay)
            {
                ShyModifier.SetVisibility(Player, 1f);
            }
            else if (timeSpan.TotalMilliseconds / 1000f < InvisDelay + InvisDelay)
            {
                timeSpan = DateTime.UtcNow - _lastMoved.AddSeconds(InvisDelay);
                var opacity = 1f - (float)timeSpan.TotalMilliseconds / 1000f / InvisDelay *
                    (100f - FinalTransparency) / 100f;
                ShyModifier.SetVisibility(Player, opacity);
            }
            else
            {
                var opacity = FinalTransparency / 100;
                ShyModifier.SetVisibility(Player, opacity);
            }
        }

        if (Player.HasDied())
        {
            ShyModifier.SetVisibility(Player, 1f);
        }
    }
}