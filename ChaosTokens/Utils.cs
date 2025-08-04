using MiraAPI.Utilities;
using UnityEngine;

namespace ChaosTokens;

public static class Utils
{
    public static LobbyNotificationMessage Notification(string text, bool negative = false)
    {
        var notif = Helpers.CreateAndShowNotification(text, negative ? Color.red : ChaosTokensPlugin.MainColor,
            spr: ChaosTokensPlugin.DiceSprite.LoadAsset());
        notif.Text.SetOutlineThickness(0.35f);
        notif.transform.localPosition = new Vector3(0f, 1f, -20f);

        return notif;
    }
}