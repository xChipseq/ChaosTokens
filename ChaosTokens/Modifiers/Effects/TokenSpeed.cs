namespace ChaosTokens.Modifiers.Effects;

public sealed class TokenSpeed(float speed) : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Speed;
    public override string ModifierName => "Token Speed";
    public override string Notification => "You are now faster!";
    public override bool Negative => false;

    public float Speed { get; private set; } = speed;
}