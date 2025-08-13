namespace ChaosTokens.Modifiers.Effects;

public sealed class TokenDrunk : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Drunk;
    public override string ModifierName => "Token Drunk";
    public override string Notification => "Your control are inverted o_0";
    public override bool Negative => true;
}