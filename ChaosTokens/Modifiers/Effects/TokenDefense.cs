namespace ChaosTokens.Modifiers.Effects;

public class TokenDefense(float duration) : TokenEffect<ProtectionModifier>(duration)
{
    public override ChaosEffects Effect => ChaosEffects.Defense;
    public override string ModifierName => "Token Defense";
    public override string Notification => $"You received protection for {duration} seconds!";
    public override bool Negative => false;
    public override bool LinkToAditional => true;
}