using TownOfUs.Modifiers.Impostor;

namespace ChaosTokens.Modifiers.Effects;

public class TokenBlackmail(byte blackmailedId) : TokenEffect<BlackmailedModifier>(blackmailedId)
{
    public override ChaosEffects Effect => ChaosEffects.Votes;
    public override string ModifierName => "Token Blackmail";
    public override string Notification => "You are blackmailed!";
    public override bool Negative => true;

    public override bool LinkToAditional => true;
}