using System.Collections.Generic;
using System.Linq;
using MiraAPI.Utilities;
using UnityEngine;

namespace ChaosTokens;

public static class Utils
{
    public static LobbyNotificationMessage Notification(string text, bool negative = false)
    {
        var notif = Helpers.CreateAndShowNotification(text, negative ? Color.red : ChaosTokensPlugin.MainColor,
            spr: Assets.DiceSprite.LoadAsset());
        notif.Text.SetOutlineThickness(0.35f);
        notif.transform.localPosition = new Vector3(0f, 1f, -20f);

        return notif;
    }

    public static SpriteRenderer CreatePostprocessFilter(Material material)
    {
        var gameObject = new GameObject("PostprocessFilter");
        gameObject.name = $"{material.name}_PostprocessFilter";
        gameObject.transform.position = new Vector3(0, 0, -50);
        gameObject.transform.localScale = new Vector3(1000, 1000, 1);

        var renderer = gameObject.AddComponent<SpriteRenderer>();
        renderer.sprite = Assets.FilterSprite.LoadAsset();
        renderer.material = material;
        return renderer;
    }

    public static List<PlayerTask> GetUncompletedTasks(PlayerControl player)
    {
        return player.myTasks
            .ToArray()
            .Where(x => x.TryCast<NormalPlayerTask>() != null && !x.IsComplete)
            .ToList();
    }
}