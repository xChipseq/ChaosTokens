using TownOfUs.Modifiers.Game.Universal;

namespace ChaosTokens.Modifiers.Effects;

public sealed class TokenTransparent(float visibility) : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Transparent;
    public override string ModifierName => "Token Transparent";
    public override string Notification => "You are now transparent!";
    public override bool Negative => false;

    public float Visibility { get; } = visibility;
    
    public override void FixedUpdate()
    {
        ShyModifier.SetVisibility(Player, Visibility, true);
    }

    public override void OnDeactivate()
    {
        ShyModifier.SetVisibility(Player, 1);
    }
}