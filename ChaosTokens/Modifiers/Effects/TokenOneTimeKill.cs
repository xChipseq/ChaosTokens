using ChaosTokens.Buttons;
using MiraAPI.Hud;

namespace ChaosTokens.Modifiers.Effects;

public class TokenOneTimeKill : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.KillButton;
    public override string ModifierName => "Token Kill Button";
    public override string Notification => "You've gained a one-time-use kill button, take things into your own hands!";
    public override bool Negative => false;

    public override void OnActivate()
    {
        base.OnActivate();
        CustomButtonSingleton<TokenKillButton>.Instance.SetTimer(10f);
        CustomButtonSingleton<TokenKillButton>.Instance.Button.gameObject.SetActive(true);
    }

    public override void OnDeactivate()
    {
        CustomButtonSingleton<TokenKillButton>.Instance.Button.gameObject.SetActive(false);
    }
}