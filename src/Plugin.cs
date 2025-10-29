using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;
using IShowSeed.Random;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.Net.Http;
using IShowSeed.Prediction;
using System.Runtime.CompilerServices;

namespace IShowSeed;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    // Internal
    internal static Plugin Instance;
    internal static ManualLogSource Beep;
    internal static HttpClient HttpClient = new();
    internal static Harmony TogglableHarmony = new($"{MyPluginInfo.PLUGIN_GUID}.togglable");
    internal static Harmony PermanentHarmony = new($"{MyPluginInfo.PLUGIN_GUID}.permanent");

    // Logic
    internal static int SeedForRandom = 0;
    private static bool OnStartupDone = false;

    // Configs
    internal static ConfigEntry<int> ConfigPresetSeed;
    internal static ConfigEntry<bool> PersistBetweenGameRestarts;
    private static ConfigEntry<string> LeaderboardUri;
    private static ConfigEntry<int> TimeoutSeconds;
    internal static ConfigEntry<string> EnabledGamemodesStr;
    internal static List<string> EnabledGamemodes;
    private static ConfigEntry<string> DesiredRouteDescription;
    private static ConfigEntry<int> SeedSearchMin;
    private static ConfigEntry<int> SeedSearchMax;

    private void Awake()
    {
        Instance = this;
        Beep = Logger;
        ConfigPresetSeed = Config.Bind("General", "PresetSeed", 0, "Preset seed, `0` to keep the default behaviour");
        PersistBetweenGameRestarts = Config.Bind("General", "PersistBetweenGameRestarts", true, "If true, the seed will stay the same even after game restart");
        EnabledGamemodesStr = Config.Bind(
            "General",
            "Gamemodes",
            $"{GamemodesList.GetAllGamemodesStr(true, "|")}",
            $"Gamemodes which should be affected by IShowSeed, separated by `|`. Available values (can append \"-Hardmode\", \"-Iron\" or \"-Hardmode-Iron\"):\n{GamemodesList.GetAllGamemodesStr(false, "\n")}"
        );
        EnabledGamemodes = [.. EnabledGamemodesStr.Value.Split('|')];
        LeaderboardUri = Config.Bind("Leaderboards", "Uri", "http://128.199.54.23:80", "If you encounter any network problems, make sure you have the latest version of the mod and reset this to default");
        TimeoutSeconds = Config.Bind("Leaderboards", "TimeoutSeconds", 15, "If this is not enough, either the server is down or you have network issues");
        DesiredRouteDescription = Config.Bind("SeedSearch", "DesiredRouteDescription", "shortcut_burner: perk_u_t2_adoptionday, perk_metabolicstasis, perk_rabbitdna", "Format: `{route name}: {unstable_perk1}, {perk2}, {perk3}`, available route names: `default`, `shortcut_sink`, `shortcut_burner`");
        SeedSearchMin = Config.Bind("SeedSearch", "MinSeed", 3665, "Min seed for desired route search");
        SeedSearchMax = Config.Bind("SeedSearch", "MaxSeed", 3665, "Max seed for desired route search, do not make this range too big or your game will load forever");

        SceneManager.sceneLoaded += OnGameStartup;
        PatchAllWithAttribute<PermanentPatchAttribute>(PermanentHarmony);
        HttpClient.BaseAddress = new Uri(LeaderboardUri.Value);
        HttpClient.Timeout = TimeSpan.FromSeconds(TimeoutSeconds.Value);
        Beep.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is loaded");
    }

    internal static void PatchAllWithAttribute<T>(Harmony harmony) where T : Attribute
    {
        var assembly = Assembly.GetExecutingAssembly();
        var patches = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<T>() != null);
        foreach (var p in patches)
        {
            harmony.PatchAll(p);
        }
    }

    public void OnGameStartup(Scene scene, LoadSceneMode _)
    {
        if (scene.name != "Main-Menu" || OnStartupDone) return;
        OnStartupDone = true;

        if (!PersistBetweenGameRestarts.Value)
        {
            ConfigPresetSeed.Value = 0;
            Beep.LogInfo("PersistBetweenGameRestarts is false, clearing the preset seed");
        }
        if (!string.IsNullOrWhiteSpace(DesiredRouteDescription.Value))
        {
            const int RequiredPerkCount = 3;
            var validRoutes = new HashSet<string> { "default", "shortcut_sink", "shortcut_burner" };
            
            var parts = DesiredRouteDescription.Value.Split(':');
            if (parts.Length == 0)
            {
                Beep.LogWarning($"Invalid DesiredRouteDescription format: '{DesiredRouteDescription.Value}'. Expected format: '{{route}}: {{perk1}}, {{perk2}}, {{perk3}}'");
                return;
            }
            
            string routeName = parts[0].Trim().ToLower();
            string[] perks = parts.Length >= 2 ? [.. parts[1].Split(',').Select(p => p.Trim().ToLower())] : [];
            if (!validRoutes.Contains(routeName))
            {
                Beep.LogWarning($"Invalid desired route name: '{routeName}'. Valid routes are: {string.Join(", ", validRoutes)}");
                return;
            }
            
            if (perks.Length != RequiredPerkCount)
            {
                Beep.LogWarning($"Invalid desired perk count: {perks.Length}. Expected exactly {RequiredPerkCount} perks but got: {string.Join(", ", perks)}");
                return;
            }

            Beep.LogInfo($"Searching desired route: {routeName}, perks: {string.Join(", ", perks)}");
            
            for (int seed = SeedSearchMin.Value; seed <= SeedSearchMax.Value; ++seed)
            {
                Vanga.RouteInfo prediction = Vanga.GenerateRouteInfos(seed).First(x => x.RouteName == routeName);
                if (prediction.PerkMachines[0].PredictedPerks.PerkIds.Contains(perks[0]) &&
                    prediction.PerkMachines[1].PredictedPerks.PerkIds.Contains(perks[1]) &&
                    prediction.PerkMachines[2].PredictedPerks.PerkIds.Contains(perks[2]))
                {
                    Beep.LogInfo($"Found suitable seed: {seed}");
                }
            }
        }
    }
}


[PermanentPatch]
[HarmonyPatch(typeof(SceneManager), "LoadScene", typeof(string))]
public static class SceneManager_LoadScene_Patcher
{
    public static void Prefix(string sceneName)
    {
        if (sceneName == "Main-Menu")
        {
            Plugin.TogglableHarmony.UnpatchSelf();
            Rod.SwitchToMode(Rod.ERandomMode.Disabled);
        }

        // apply patches before the first Awake but after the gamemode is known
        if (sceneName == "Game-Main")
        {
            if (Plugin.EnabledGamemodes.Contains(CL_GameManager.GetGamemodeName()))
            {
                if (Rod.GetMode() == Rod.ERandomMode.Enabled)
                {
                    Plugin.Beep.LogInfo("Only resetting random on restart, no double patching");
                    Rod.Reset();
                }
                else
                {
                    Plugin.Beep.LogInfo("Applying patches for random for the first time");
                    Rod.SwitchToMode(Rod.ERandomMode.Enabled);
                    Plugin.PatchAllWithAttribute<TogglablePatchAttribute>(Plugin.TogglableHarmony);
                }
            }
            else
            {
                Plugin.Beep.LogInfo($"Gamemode `{CL_GameManager.GetGamemodeName()}` is not enabled in IShowSeed's configs, not doing anything");
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class TogglablePatchAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class PermanentPatchAttribute : Attribute { }


internal static class GamemodesList
{
    internal static List<string> BaseGamemodes = [
        "Campaign",
        "Endless Superstructure",
        "Endless Substructure",
        "Endless Underworks",
        "Endless Silos",
        "Endless Pipeworks",
        "Endless Habitation",
        "Endless Abyss",
    ];

    internal static List<string> GetAllGamemodes(bool withOptions)
    {
        List<string> res = [];
        foreach (var g in BaseGamemodes)
        {
            res.Add(g);
            if (withOptions)
            {
                res.Add(g + "-Hardmode");
                res.Add(g + "-Iron");
                res.Add(g + "-Hardmode-Iron");
            }
        }
        return res;
    }

    internal static string GetAllGamemodesStr(bool withOptions, string delimiter)
    {
        return String.Join(delimiter, GetAllGamemodes(withOptions));
    }

}
