using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;

namespace IShowSeed;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class IShowSeedPlugin : BaseUnityPlugin
{
    internal static IShowSeedPlugin Instance;
    internal static ManualLogSource Beep;
    private readonly Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    internal static int StartingSeed = 0;
    internal static ConfigEntry<int> configPresetSeed;

    private void Awake()
    {
        Instance = this;
        Beep = base.Logger;
        configPresetSeed = Config.Bind("General", "PresetSeed", 0, "Preset seed to use in all gamemodes, `0` to keep the default behaviour");
        Harmony.PatchAll();
        Beep.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is loaded");
    }
}

