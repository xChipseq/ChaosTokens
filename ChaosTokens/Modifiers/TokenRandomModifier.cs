using System;
using System.Linq;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public sealed class TokenRandomModifier : BaseModifier
{
    public override string ModifierName => "Token Random Modifier";
    public override bool HideOnUi => true;

    private Type ModifierType;

    public override void OnActivate()
    {
        if (Player.AmOwner)
        {
            Utils.Notification("<b>Your received a random modifier!</b>");
        }

        if (!AmongUsClient.Instance.AmHost) return;

        var modifier = typeof(TownOfUsPlugin).Assembly
            .GetTypes()
            .Where(x => typeof(GameModifier).IsAssignableFrom(x))
            .Where(x => !x.IsAbstract)
            .Where(x => !x.IsDefined(typeof(MiraIgnoreAttribute), false))
            .Where(x =>
            {
                var temp = Activator.CreateInstance(x) as GameModifier;
                return temp.IsModifierValidOn(Player.Data.Role);
            })
            .Where(x => !Player.HasModifier(x))
            .Random();
        
        Player.RpcAddModifier(modifier);
        ModifierType = modifier;
    }

    public override void OnDeactivate()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (ModifierType == null) return;
        
        Player.RpcRemoveModifier(ModifierType);
    }
}