using ChaosTokens.Buttons;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public class ChaosTokenModifier(int amount = 1, bool showNotification = true) : BaseModifier
{
    public override string ModifierName => $"Chaos Token{(Tokens > 1 ? "s" : string.Empty)}";
    public override LoadableAsset<Sprite> ModifierIcon => Assets.DiceSprite;
    public override string GetDescription() => $"Take your chances!\n<b>Tokens left: {Tokens}</b>";

    public int Tokens { get; private set; } = 0;

    public override void OnActivate()
    {
        Tokens = amount;

        if (Player.AmOwner)
        {
            CustomButtonSingleton<RollButton>.Instance.ResetCooldownAndOrEffect();
            
            if (showNotification)
                TokensReceived(amount);
        }
    }

    public override void OnDeactivate()
    {
        if (Player.AmOwner)
        {
            CustomButtonSingleton<RollButton>.Instance?.Button.gameObject.SetActive(false);
        }
    }

    public void IncreaseTokens(int amount, bool showNotification = true)
    {
        Tokens += amount;
        if (Player.AmOwner && showNotification)
            TokensReceived(amount);
    }
    
    public void DecreaseTokens(int amount)
    {
        Tokens -= amount;
    }

    private void TokensReceived(int amount)
    {
        Utils.Notification($"<b>You received {amount} token{(amount > 1 ? "s" : string.Empty)}!</b>");
    }

    public override void FixedUpdate()
    {
        if (Tokens <= 0)
        {
            Player.RemoveModifier(this);
        }
    }
}