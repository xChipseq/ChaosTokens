using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using UnityEngine;

namespace ChaosTokens.Options;

public class GeneralOptions : AbstractOptionGroup
{
    public override string GroupName => "<b>General<b>";
    public override Color GroupColor => ChaosTokensPlugin.MainColor;
    public override uint GroupPriority => 1;
    
    
    [ModdedToggleOption("Enable Chaos Tokens")]
    public bool EnableChaosTokens { get; set; } = true;
    
    [ModdedNumberOption("Start Roll Cooldown", 0, 60, 2.5f, MiraNumberSuffixes.Seconds, formatString: "0.0")]
    public float InitialRollCooldown { get; set; } = 10f;
    
    [ModdedNumberOption("Roll Cooldown", 5, 60, 2.5f, MiraNumberSuffixes.Seconds, formatString: "0.0")]
    public float RollCooldown { get; set; } = 5f;
}