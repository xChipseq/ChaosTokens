using System.Linq;
using HarmonyLib;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using TownOfUs.Assets;
using TownOfUs.Modules;
using UnityEngine;

namespace ChaosTokens.Modifiers;

public class TokenDeath : BaseModifier
{
    public override string ModifierName => "Token Death";
    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        if (!Player.AmOwner) return;
        
        Utils.Notification("<b>Count your days, you will die at the end of the next meeting!</b>", true);
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        var playerPva = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == Player.PlayerId);
        var icon = GameObject.Instantiate(playerPva.XMark, playerPva.XMark.transform.parent);
        icon.name = "TokenDeathIcon";
        icon.transform.SetLocalZ(-5);
        icon.transform.localPosition += new Vector3(0.1f, -0.05f);
        icon.gameObject.SetActive(true);
        icon.sprite = ChaosTokensPlugin.TokenDeathSprite.LoadAsset();
        icon.color = Color.red;
        
        MeetingMenu.Instances.Do(x => x.HideSingle(Player.PlayerId));
    }
}