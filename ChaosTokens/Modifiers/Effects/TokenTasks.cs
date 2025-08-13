using System;
using System.Linq;
using System.Text.RegularExpressions;
using MiraAPI.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;
using StringBuilder = Il2CppSystem.Text.StringBuilder;

namespace ChaosTokens.Modifiers.Effects;

public class TokenTasks : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Tasks;
    public override string ModifierName => "Token Tasks";
    public override bool Negative => false;

    public override void OnActivate()
    {
        base.OnActivate();
        
        if (Player.AmOwner && Player.myTasks.Count > 0 && !Player.HasDied())
        {
            var tasks = Utils.GetUncompletedTasks(Player);

            for (int i = 0; i < Random.RandomRangeInt(1, 3); i++)
            {
                if (tasks.Count <= 0) break;

                tasks.Shuffle();

                var randomTask = tasks[0];

                HudManager.Instance.ShowTaskComplete();
                Player.RpcCompleteTask(randomTask.Id);

                var sb = new StringBuilder();
                randomTask.AppendTaskText(sb);

                var pattern = @" \(.*?\)";
                var query = sb.ToString();
                var taskText = Regex.Replace(query, pattern, string.Empty);
                taskText = taskText.Replace(Environment.NewLine, "");

                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{ChaosTokensPlugin.MainColor.ToTextColor()}The task '{taskText}' has been completed for you.</b></color>",
                    Color.white, spr: Assets.DiceSprite.LoadAsset());
                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            }
        }
    }
}