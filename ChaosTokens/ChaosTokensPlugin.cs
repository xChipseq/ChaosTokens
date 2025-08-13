using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MiraAPI.PluginLoading;
using Reactor;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfUs;
using UnityEngine;

namespace ChaosTokens;

[BepInAutoPlugin("chipseq.chaostokens", "ChaosTokens")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(TownOfUsPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class ChaosTokensPlugin : BasePlugin, IMiraPlugin
{
    public static bool DevBuild => false; // enable for some cool features when testing
    public Harmony Harmony { get; } = new(Id);
    public ConfigFile GetConfigFile() => Config;
    public string OptionsTitleText => "Chaos\nTokens";
    
    public static Color MainColor => new Color32(221, 178, 68, 255);

    public override void Load()
    {
        Harmony.PatchAll();
        ReactorCredits.Register<ChaosTokensPlugin>(ReactorCredits.AlwaysShow);

        if (DevBuild)
        {
            AddComponent<TokenDebugWindow>();
        }
    }
}