using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;
using UnityEngine.SceneManagement;
using IShowSeed.Random;
using System.Collections.Generic;
using IShowSeed.Prediction;

namespace IShowSeed;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    // Internal
    internal static Plugin Instance;
    internal static ManualLogSource Beep;
    internal static Harmony TogglableHarmony = new($"{MyPluginInfo.PLUGIN_GUID}.togglable");
    internal static Harmony PermanentHarmony = new($"{MyPluginInfo.PLUGIN_GUID}.permanent");

    // Logic
    internal static int SeedForRandom = 0;
    private static bool OnStartupDone = false;

    // Configs
    internal static ConfigEntry<int> ConfigPresetSeed;
    internal static ConfigEntry<bool> PersistBetweenGameRestarts;
    internal static ConfigEntry<string> LeaderboardUri;
    internal static ConfigEntry<int> TimeoutSeconds;
    internal static ConfigEntry<string> EnabledGamemodesStr;
    internal static List<string> EnabledGamemodes;
    internal static ConfigEntry<string> DesiredRouteDescription;
    internal static ConfigEntry<int> SeedSearchMin;
    internal static ConfigEntry<int> SeedSearchMax;

    private void Awake()
    {
        Instance = this;
        Beep = Logger;
        ConfigPresetSeed = Config.Bind("General", "PresetSeed", 0, "Preset seed, `0` to keep the default behaviour");
        PersistBetweenGameRestarts = Config.Bind("General", "PersistBetweenGameRestarts", true, "If true, the seed will stay the same even after game restart");
        EnabledGamemodesStr = Config.Bind(
            "General",
            "Gamemodes",
            $"{Helpers.GetAllGamemodesStr(true, "|")}",
            $"Gamemodes which should be affected by IShowSeed, separated by `|`. Available values (can append \"-Hardmode\", \"-Iron\" or \"-Hardmode-Iron\"):\n{Helpers.GetAllGamemodesStr(false, "\n")}"
        );
        EnabledGamemodes = [.. EnabledGamemodesStr.Value.Split('|')];
        LeaderboardUri = Config.Bind("Leaderboards", "Uri", "http://128.199.54.23:80", "If you encounter any network problems, make sure you have the latest version of the mod and reset this to default");
        TimeoutSeconds = Config.Bind("Leaderboards", "TimeoutSeconds", 10, "If this is not enough, either the server is down or you have network issues");
        DesiredRouteDescription = Config.Bind("SeedSearch", "DesiredRouteDescription", "shortcut_burner: perk_u_t2_adoptionday, perk_metabolicstasis, perk_rabbitdna", "Format: `{route name}: {unstable_perk1}, {perk2}, {perk3}`, available route names: `default`, `shortcut_sink`, `shortcut_burner`");
        SeedSearchMin = Config.Bind("SeedSearch", "MinSeed", 3665, "Min seed for desired route search");
        SeedSearchMax = Config.Bind("SeedSearch", "MaxSeed", 3665, "Max seed for desired route search, do not make this range too big or your game will load forever");

        SceneManager.sceneLoaded += OnGameStartup;
        Beep.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is loaded");
    }

    public void OnGameStartup(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name != "Main-Menu" || OnStartupDone) return;
        OnStartupDone = true;

        _ = MasterServer.InitializeHttpClient();
        ClearSeedOnRestartIfNeeded();
        Vanga.DoSeedSearch();
        Helpers.PatchAllWithAttribute<PermanentPatchAttribute>(PermanentHarmony);
    }

    public void ClearSeedOnRestartIfNeeded()
    {
        if (!PersistBetweenGameRestarts.Value)
        {
            ConfigPresetSeed.Value = 0;
            Beep.LogInfo("PersistBetweenGameRestarts is false, clearing the preset seed");
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
                    Helpers.PatchAllWithAttribute<TogglablePatchAttribute>(Plugin.TogglableHarmony);
                }
            }
            else
            {
                Plugin.Beep.LogInfo($"Gamemode `{CL_GameManager.GetGamemodeName()}` is not enabled in IShowSeed's configs, not doing anything");
            }
        }
    }
}

