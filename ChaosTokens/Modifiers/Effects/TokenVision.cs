namespace ChaosTokens.Modifiers.Effects;

public class TokenVision(float mult) : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Vision;
    public override string ModifierName => "Token Vision";
    public override string Notification => "You now have increased vision!";
    public override bool Negative => false;

    public float Multiplier = mult;
}