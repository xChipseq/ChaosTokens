using System.Linq;
using HarmonyLib;
using TownOfUs.Modules;
using UnityEngine;

namespace ChaosTokens.Modifiers.Effects;

public class TokenDeath : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.Death;
    public override string ModifierName => "Token Death";
    public override string Notification => "Count your days, you will die at the end of the next meeting!";
    public override bool Negative => true;
    public override bool RemoveOnDeath => true;
    
    public override void OnMeetingStart()
    {
        if (Player.AmOwner)
        {
            return;
        }
        var playerPva = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == Player.PlayerId);
        var icon = GameObject.Instantiate(playerPva.XMark, playerPva.XMark.transform.parent);
        icon.name = "TokenDeathIcon";
        icon.transform.SetLocalZ(-5);
        icon.transform.localPosition += new Vector3(0.1f, -0.05f);
        icon.gameObject.SetActive(true);
        icon.sprite = Assets.TokenDeathSprite.LoadAsset();
        icon.color = Color.red;
    }

    public override void Update()
    {
        if (MeetingHud.Instance && !Player.AmOwner)
        {
            MeetingMenu.Instances.Do(x => x.HideSingle(Player.PlayerId));
        }
    }
}