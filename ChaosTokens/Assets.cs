using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using UnityEngine;

namespace ChaosTokens;

public static class Assets
{
    public static AssetBundle ShaderBundle = AssetBundleManager.Load("tokenshader");
    
    public static LoadableAsset<Sprite> DiceButton { get; } = new LoadableResourceAsset("ChaosTokens.Resources.DiceButton.png"); 
    public static LoadableAsset<Sprite> DiceSprite { get; } = new LoadableResourceAsset("ChaosTokens.Resources.Dice.png"); 
    public static LoadableAsset<Sprite> TokenDeathSprite { get; } = new LoadableResourceAsset("ChaosTokens.Resources.TokenDeath.png", 300);
    public static LoadableAsset<Sprite> FilterSprite { get; } = new LoadableResourceAsset("ChaosTokens.Resources.Filter.png"); 
    
    public static LoadableBundleAsset<Material> NauseaMaterial = new("NauseaMaterial", ShaderBundle);
    public static LoadableBundleAsset<Material> ColorblindMaterial = new("ColorblindMaterial", ShaderBundle);
}