using System;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using UnityEngine;

namespace ChaosTokens.Options;

public class TokenHandingOptions : AbstractOptionGroup
{
    public override string GroupName => "<b>Token Handing<b>";
    public override Color GroupColor => new Color32(47, 168, 100, 255);
    public override uint GroupPriority => 2;
    public override Func<bool> GroupVisible => () => OptionGroupSingleton<GeneralOptions>.Instance.EnableChaosTokens;


    [ModdedToggleOption("Hand Tokens First Round")]
    public bool TokensEnabledFirstRound { get; set; } = true;
    
    [ModdedToggleOption("Enable Token Handing Priority")]
    public bool TokenHandingPriority { get; set; } = true;
    
    [ModdedNumberOption("Handed Tokens Min", 1, 30)]
    public float TokensMin { get; set; } = 5;
    
    [ModdedNumberOption("Handed Tokens Max", 0, 30, zeroInfinity: true)]
    public float TokensMax { get; set; } = 10;
    
    [ModdedNumberOption("Initial Double Token Chance", 0, 100, 10, MiraNumberSuffixes.Percent)]
    public float InitialDoubleTokenChance { get; set; } = 10;
    
    [ModdedNumberOption("Double Token Chance Increase Per Round", 5, 50, 5, MiraNumberSuffixes.Percent)]
    public float DoubleTokenIncrease { get; set; } = 10;
}