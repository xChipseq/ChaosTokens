using AmongUs.GameOptions;
using HarmonyLib;
using TownOfUs.Modules;
using TownOfUs.Utilities;

namespace ChaosTokens.Modifiers.Effects;

public class TokenReveal(RoleTypes role, byte source) : TokenEffect<RoleReveal>(role, source)
{
    public override ChaosEffects Effect => ChaosEffects.RevealSelf;
    public override string ModifierName => "Token Reveal";
    public override bool Negative => false;
    public override bool RemoveAfterMeeting => false;
    public override bool LinkToAditional => true;
    
    public bool FakeReveal { get; protected set; }
    private bool _ghostInfoUpdated = false;
    
    public override void OnActivate()
    {
        base.OnActivate();

        FakeReveal = !Player.Is(role);
        AdditionalModifier.RevealSource = MiscUtils.PlayerById(source);
        
        if (Player.AmOwner && FakeReveal)
        {
            Utils.Notification("<b>You have been revealed to everyone as a random role!</b>");
            AdditionalModifier.ExtraRoleText = " <size=50%><i><color=red>(fake)</color></i></size>";
            return;
        }
        
        if (Player.AmOwner)
        {
            Utils.Notification("<b>Your role has been revealed to everyone!</b>");
            AdditionalModifier.ExtraRoleText = " <size=50%><i><color=red>(revealed)</color></i></size>";
            return;
        }
        
        if (AdditionalModifier?.RevealSource?.AmOwner ?? false)
        {
            Utils.Notification("<b>You revealed the role of a random person!</b>");
        }
    }
    
    public override void Update()
    {
        if (MeetingHud.Instance && !Player.AmOwner)
        {
            MeetingMenu.Instances.Do(x => x.HideSingle(Player.PlayerId));
        }

        if (!_ghostInfoUpdated && FakeReveal && PlayerControl.LocalPlayer.HasDied())
        {
            _ghostInfoUpdated = true;
            AdditionalModifier.ExtraRoleText = $" <size=50%><i><color=red>(revealed as {AdditionalModifier.ShownRole?.NiceName})</color></i></size>";
            AdditionalModifier.ShownRole = Player.GetRoleWhenAlive();
        }
    }
}