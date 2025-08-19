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
    
    public override void OnActivate()
    {
        base.OnActivate();

        FakeReveal = !Player.Is(role);
        
        if (Player.AmOwner && FakeReveal)
        {
            Utils.Notification("<b>You have been revealed to everyone as a random role!</b>");
            
            AdditionalModifier.ExtraRoleText = " <i><color=red>(fake)</color></i>";
            return;
        }
        
        if (Player.AmOwner)
        {
            Utils.Notification("<b>Your role has been revealed to everyone!</b>");
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
    }
}