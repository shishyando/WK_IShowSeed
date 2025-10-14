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

namespace IShowSeed;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class IShowSeedPlugin : BaseUnityPlugin
{
    internal static IShowSeedPlugin Instance;
    internal static ManualLogSource Beep;
    internal static Harmony TogglableHarmony = new($"{MyPluginInfo.PLUGIN_GUID}.togglable");
    internal static Harmony PermanentHarmony = new($"{MyPluginInfo.PLUGIN_GUID}.permanent");

    internal static int StartingSeed = 0;
    internal static ConfigEntry<int> ConfigPresetSeed;
    internal static ConfigEntry<bool> PersistBetweenGameRestarts;
    internal static ConfigEntry<string> EnabledGamemodesStr;
    internal static List<string> EnabledGamemodes;

    private void Awake()
    {
        Instance = this;
        Beep = Logger;
        ConfigPresetSeed = Config.Bind("General", "PresetSeed", 0, "Preset seed, `0` to keep the default behaviour");
        EnabledGamemodesStr = Config.Bind(
            "General",
            "Gamemodes",
            $"{GamemodesList.GetAllGamemodesStr(true, "|")}",
            $"Gamemodes which should be affected by IShowSeed, separated by `|`. Available values (can append \"-Hardmode\", \"-Iron\" or \"-Hardmode-Iron\"):\n{GamemodesList.GetAllGamemodesStr(false, "\n")}"
        );
        PersistBetweenGameRestarts = Config.Bind("General", "PersistBetweenGameRestarts", true, "If true, the seed will stay the same even after game restart");

        EnabledGamemodes = [.. EnabledGamemodesStr.Value.Split('|')];

        PatchAllWithAttribute<PermanentPatchAttribute>(PermanentHarmony);

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
}


[PermanentPatch]
[HarmonyPatch(typeof(SceneManager), "LoadScene", typeof(string))]
public static class SceneManager_LoadScene_Patcher
{
    public static void Prefix(string sceneName)
    {
        if (sceneName == "Intro" && !IShowSeedPlugin.PersistBetweenGameRestarts.Value)
        {
            IShowSeedPlugin.ConfigPresetSeed.Value = 0;
            IShowSeedPlugin.Beep.LogInfo("PersistBetweenGameRestarts is false, clearing the preset seed");
        }
        if (sceneName == "Main-Menu")
        {
            IShowSeedPlugin.TogglableHarmony.UnpatchSelf();
            Rod.SwitchToMode(Rod.ERandomMode.Disabled);
        }
        
        // apply patches before the first Awake but after the gamemode is known
        if (sceneName == "Game-Main")
        {
            if (IShowSeedPlugin.EnabledGamemodes.Contains(CL_GameManager.GetGamemodeName()))
            {
                if (Rod.GetMode() == Rod.ERandomMode.Enabled)
                {
                    IShowSeedPlugin.Beep.LogInfo("Only resetting random on restart, no double patching");
                    Rod.Reset();
                }
                else
                {
                    IShowSeedPlugin.Beep.LogInfo("Applying patches for random for the first time");
                    Rod.SwitchToMode(Rod.ERandomMode.Enabled);
                    IShowSeedPlugin.PatchAllWithAttribute<TogglablePatchAttribute>(IShowSeedPlugin.TogglableHarmony);
                }
            }
            else
            {
                IShowSeedPlugin.Beep.LogInfo($"Gamemode `{CL_GameManager.GetGamemodeName()}` is not enabled in IShowSeed's configs, not doing anything");
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
