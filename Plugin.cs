using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using BepInEx.Configuration;

namespace IShowSeed;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class IShowSeedPlugin : BaseUnityPlugin
{
    internal static IShowSeedPlugin Instance;
    internal static new ManualLogSource Logger;
    private readonly Harmony Harmony = new(MyPluginInfo.PLUGIN_GUID);

    internal static int StartingSeed = 0;
    internal static ConfigEntry<int> configPresetSeed;

    private void Awake()
    {
        Instance = this;
        Logger = base.Logger;
        configPresetSeed = Config.Bind("General", "PresetSeed", 0, "Preset seed to use in all gamemodes, `0` to keep the default behaviour");
        Harmony.PatchAll(typeof(Patches.WorldLoader_Awake_Patcher));
        Harmony.PatchAll(typeof(Patches.WorldLoader_Initialize_Patcher));
        Harmony.PatchAll(typeof(Patches.CL_UIManager_Update_Patcher));
        Harmony.PatchAll(typeof(Patches.MenuManager_Start_Patcher));
        Harmony.PatchAll(typeof(Patches.UT_SeededEnable_OnEnable_Patcher));
        Harmony.PatchAll(typeof(Patches.ENV_VendingMachine_SetSeed_Patcher));
        Harmony.PatchAll(typeof(Patches.UT_SpawnChance_Start_Patcher));
        Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} is loaded");
    }
}

