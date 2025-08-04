using MiraAPI.Modifiers;
using MiraAPI.Utilities;

namespace ChaosTokens.Modifiers;

public class TokenVotes : BaseModifier
{
    public override string ModifierName => "Token Votes";
    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        if (!Player.AmOwner) return;

        Utils.Notification("<b>You will get a random amount of additional votes the next meeting!</b>");
    }
}