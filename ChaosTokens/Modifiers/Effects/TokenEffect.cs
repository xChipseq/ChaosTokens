using MiraAPI.Modifiers;
using MiraAPI.PluginLoading;

namespace ChaosTokens.Modifiers.Effects;

[MiraIgnore]
public abstract class TokenEffect : BaseModifier
{
    public abstract ChaosEffects Effect { get; }
    public abstract bool Negative { get; }
    public virtual string Notification { get; }
    public virtual bool RemoveAfterMeeting => true;
    public virtual bool RemoveOnDeath => false;

    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        if (!Player.AmOwner) return;

        if (!string.IsNullOrEmpty(Notification))
        {
            Utils.Notification($"<b>{Notification}</b>", Negative);
        }
    }
    
    public override void OnDeath(DeathReason reason)
    {
        if (RemoveOnDeath)
        {
            Player.RemoveModifier(this);
        }
    }
}

[MiraIgnore]
public abstract class TokenEffect<T>(params object[] args) : TokenEffect where T : BaseModifier
{
    public abstract bool LinkToAditional { get; }
    
    public T AdditionalModifier { get; protected set; }
    private bool AdditionalHanded { get; set; }

    public virtual void OnAdditionalDeactivate()
    {
    }
    
    public override void OnActivate()
    {
        base.OnActivate();
        
        AdditionalModifier = Player.AddModifier<T>(args);
        AdditionalHanded = true;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        
        if (AdditionalModifier != null)
        {
            Player.RemoveModifier(AdditionalModifier);
        }
    }

    public override void FixedUpdate()
    {
        if (AdditionalHanded && AdditionalModifier == null)
        {
            OnAdditionalDeactivate();
            
            if (LinkToAditional)
            {
                Player.RemoveModifier(this);
            }
        }
    }
}