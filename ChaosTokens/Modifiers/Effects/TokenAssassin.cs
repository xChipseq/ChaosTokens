using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using Reactor.Utilities;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Events;
using TownOfUs.Extensions;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Options;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;

namespace ChaosTokens.Modifiers.Effects;

public class TokenAssassin : TokenEffect
{
    public override string ModifierName => "Token Assassin";
    public override string Notification => "You can attempt to guess a player's role during the next meeting.";
    public override ChaosEffects Effect => ChaosEffects.Assassin;
    public override bool Negative => false;
    public override bool RemoveAfterMeeting => true;

    private MeetingMenu meetingMenu;
    public string LastGuessedItem { get; set; }
    [HideFromIl2Cpp]
    public PlayerControl LastAttemptedVictim { get; set; }

    public override void OnActivate()
    {
        base.OnActivate();

        //Logger<TownOfUsPlugin>.Error($"AssassinModifier.OnActivate maxKills: {maxKills}");
        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                Player.Data.Role,
                ClickGuess,
                MeetingAbilityType.Click,
                TouAssets.Guess,
                null!,
                IsExempt);
        }
    }

    public override void OnMeetingStart()
    {
        base.OnMeetingStart();
        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance,
                Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());
        }
    }

    public void OnVotingComplete()
    {
        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud meetingHud)
    {
        if (meetingHud.state == MeetingHud.VoteStates.Discussion)
        {
            return;
        }

        if (Minigame.Instance != null)
        {
            return;
        }

        var player = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        var shapeMenu = GuesserMenu.Create();
        shapeMenu.Begin(IsRoleValid, ClickRoleHandle, IsModifierValid, ClickModifierHandle);

        void ClickRoleHandle(RoleBehaviour role)
        {
            var realRole = player.Data.Role;

            var cachedMod = player.GetModifiers<BaseModifier>().FirstOrDefault(x => x is ICachedRole) as ICachedRole;
            if (cachedMod != null)
            {
                realRole = cachedMod.CachedRole;
            }

            var pickVictim = role.Role == realRole.Role;
            var victim = pickVictim ? player : Player;

            ClickHandler(victim);
            LastAttemptedVictim = player;
            LastGuessedItem = $"{role.TeamColor.ToTextColor()}{role.NiceName}</color>";
        }

        void ClickModifierHandle(BaseModifier modifier)
        {
            var pickVictim = player.HasModifier(modifier.TypeId);
            var victim = pickVictim ? player : Player;

            ClickHandler(victim);
            LastAttemptedVictim = player;
            LastGuessedItem =
                $"{MiscUtils.GetRoleColour(modifier.ModifierName.Replace(" ", string.Empty)).ToTextColor()}{modifier.ModifierName}</color>";
        }

        void ClickHandler(PlayerControl victim)
        {
            if (victim != Player && victim.TryGetModifier<OracleBlessedModifier>(out var oracleMod))
            {
                OracleRole.RpcOracleBlessNotify(oracleMod.Oracle, PlayerControl.LocalPlayer, victim);

                MeetingMenu.Instances.Do(x => x.HideSingle(victim.PlayerId));

                shapeMenu.Close();
                LastGuessedItem = string.Empty;
                LastAttemptedVictim = null;
                Utils.Notification("Your target was blessed.");
                Player.RpcRemoveModifier<TokenAssassin>();
                return;
            }

            if (victim == Player)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfUsColors.Impostor));
                Utils.Notification("Your guess was incorrect.");
                shapeMenu.Close();
                LastGuessedItem = string.Empty;
                LastAttemptedVictim = null;
                Player.RpcRemoveModifier<TokenAssassin>();
                return;
            }

            Player.RpcCustomMurder(victim, createDeadBody: false, teleportMurderer: false, showKillAnim: false,
                playKillSound: false);

            if (victim != Player)
            {
                LastGuessedItem = string.Empty;
                LastAttemptedVictim = null;
                MeetingMenu.Instances.Do(x => x.HideSingle(victim.PlayerId));
                DeathHandlerModifier.RpcUpdateDeathHandler(victim, "Guessed", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, $"By {Player.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);
            }

            shapeMenu.Close();
            meetingMenu!.HideButtons();
            Player.RpcRemoveModifier<TokenAssassin>();
            Utils.Notification("You have guessed correctly.");
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        var votePlayer = voteArea.GetPlayer();
        return voteArea?.TargetPlayerId == Player.PlayerId ||
               Player.Data.IsDead ||
               voteArea!.AmDead ||
               (Player.IsImpostor() && votePlayer?.IsImpostor() == true &&
                !OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode) ||
               (Player.Data.Role is VampireRole && votePlayer?.Data.Role is VampireRole) ||
               (votePlayer?.Data.Role is MayorRole mayor && mayor.Revealed) ||
               (votePlayer?.GetModifiers<RevealModifier>().Any(x => x.Visible && x.RevealRole) == true) ||
               (Player.IsLover() && votePlayer?.IsLover() == true) ||
               votePlayer?.HasModifier<JailedModifier>() == true;
    }

    private bool IsRoleValid(RoleBehaviour role)
    {
        if (role.IsDead)
        {
            return false;
        }

        var touRole = role as ITownOfUsRole;
        var unguessableRole = role as IUnguessable;

        if (touRole is IGhostRole || unguessableRole is { IsGuessable: false })
        {
            return false;
        }

        return true;
    }

    private static bool IsModifierValid(BaseModifier modifier)
    {
        var isValid = true;
        // This will remove modifiers that alter their chance/amount
        if ((modifier is TouGameModifier touMod && (touMod.CustomAmount <= 0 || touMod.CustomChance <= 0)) ||
            (modifier is AllianceGameModifier allyMod && (allyMod.CustomAmount <= 0 || allyMod.CustomChance <= 0)) ||
            (modifier is UniversalGameModifier uniMod && (uniMod.CustomAmount <= 0 || uniMod.CustomChance <= 0)))
        {
            isValid = false;
        }

        if (!isValid)
        {
            return false;
        }

        if (OptionGroupSingleton<AssassinOptions>.Instance.AssassinGuessAlliances &&
            modifier is AllianceGameModifier)
        {
            return true;
        }

        if (!OptionGroupSingleton<AssassinOptions>.Instance.AssassinGuessCrewModifiers)
        {
            return false;
        }

        if (!OptionGroupSingleton<AssassinOptions>.Instance.AssassinGuessUtilityModifiers &&
            modifier is TouGameModifier touMod2 && touMod2.FactionType == ModifierFaction.CrewmateUtility)
        {
            return false;
        }

        var crewMod = modifier as TouGameModifier;
        if (crewMod != null && crewMod.FactionType.ToDisplayString().Contains("Crew") &&
            !crewMod.FactionType.ToDisplayString().Contains("Non"))
        {
            return true;
        }

        return false;
    }
}