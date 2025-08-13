namespace ChaosTokens.Modifiers.Effects;

public class TokenVotes : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Votes;
    public override string ModifierName => "Token Votes";
    public override string Notification => "You will get a random amount of additional votes the next meeting!";
    public override bool Negative => false;
}