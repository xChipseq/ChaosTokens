using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using UnityEngine;

namespace ChaosTokens;

public class ChaosTokensOptions : AbstractOptionGroup
{
    public override string GroupName => "<color=#ffd91c><b>Chaos Tokens<b></color>";
    public override Color GroupColor => ChaosTokensPlugin.MainColor;
    
    [ModdedToggleOption("Enable Chaos Tokens")]
    public bool EnableChaosTokens { get; set; } = true;
    
    [ModdedNumberOption("Start Roll Cooldown", 0, 60, 2.5f, MiraNumberSuffixes.Seconds, formatString: "0.0")]
    public float InitialRollCooldown { get; set; } = 10f;
    
    [ModdedNumberOption("Roll Cooldown", 5, 60, 2.5f, MiraNumberSuffixes.Seconds, formatString: "0.0")]
    public float RollCooldown { get; set; } = 5f;
    
    
    [ModdedNumberOption("Handed Tokens Min", 1, 15)]
    public float TokensMin { get; set; } = 5;
    
    [ModdedNumberOption("Handed Tokens Max", 0, 15, zeroInfinity: true)]
    public float TokensMax { get; set; } = 10;
    
    [ModdedNumberOption("Initial Double Token Chance", 0, 100, 10, MiraNumberSuffixes.Percent)]
    public float InitialDoubleTokenChance { get; set; } = 10;
    
    [ModdedNumberOption("Double Token Chance Increase Per Round", 5, 50, 5, MiraNumberSuffixes.Percent)]
    public float DoubleTokenIncrease { get; set; } = 10;
}