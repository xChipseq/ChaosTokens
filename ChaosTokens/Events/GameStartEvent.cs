using ChaosTokens.Options;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;

namespace ChaosTokens.Events;

public static class GameStartEvent
{
    [RegisterEvent]
    public static void GameStartEventRegister(IntroBeginEvent @event)
    {
        ChaosTokensRpc.RevealsLeft = (int)OptionGroupSingleton<BalanceOptions>.Instance.MaxRoleReveals;
    }
}