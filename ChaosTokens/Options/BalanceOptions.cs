using System;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using UnityEngine;

namespace ChaosTokens.Options;

public class BalanceOptions : AbstractOptionGroup
{
    public override string GroupName => "<b>Balance<b>";
    public override Color GroupColor => new Color32(42, 113, 184, 255);
    public override uint GroupPriority => 3;
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<GeneralOptions>.Instance.EnableChaosTokens;

    [ModdedToggleOption("Disable Death Token")]
    public bool DeathDisabled { get; set; } = false;
    
    [ModdedToggleOption("Disable Revival")]
    public bool ReviveDisabled { get; set; } = false;
    
    /*
    [ModdedToggleOption("Disable Screen Effects")]
    public bool ScreenEffectsDisabled { get; set; } = false;
    */
    
    [ModdedNumberOption("Max Role Reveals", 0, 15, zeroInfinity: true)]
    public float MaxRoleReveals { get; set; } = 0;
}