using Reactor.Utilities.Extensions;
using TMPro;

namespace ChaosTokens.Modifiers.Effects;

public class TokenNoSkip : TokenEffect
{
    public override ChaosEffects Effect => ChaosEffects.NoSkip;
    public override string ModifierName => "Token No Skip";
    public override string Notification => "The next meeting cannot be skipped!";
    public override bool Negative => false;

    public override void OnMeetingStart()
    {
        var instance = MeetingHud.Instance;
        instance.SkippedVoting.SetActive(true);
        var text = instance.SkippedVoting.GetComponentInChildren<TextMeshPro>();
        text.gameObject.GetComponent<TextTranslatorTMP>().DestroyImmediate();
        text.text = "SKIP DISABLED";
    }

    public override void Update()
    {
        if (MeetingHud.Instance)
        {
            MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(false);
        }
    }
}