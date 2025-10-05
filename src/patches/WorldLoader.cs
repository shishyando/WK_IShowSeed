using HarmonyLib;
using IShowSeed.Random;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(WorldLoader), "Awake")]
public static class WorldLoader_Awake_Patcher
{
    public static void Prefix()
    {
        WorldLoader.SetPresetSeed(IShowSeedPlugin.configPresetSeed.Value.ToString());
        IShowSeedPlugin.Beep.LogInfo($"custom preset seed: {IShowSeedPlugin.configPresetSeed.Value}");
        IShowSeedPlugin.StartingSeed = IShowSeedPlugin.configPresetSeed.Value;
    }
}


[HarmonyPatch(typeof(WorldLoader), "Initialize")]
public static class WorldLoader_Initialize_Patcher
{
    public static void Prefix()
    {
        IShowSeedPlugin.Beep.LogInfo($"World initializing with seed: {IShowSeedPlugin.configPresetSeed.Value}, resetting random...");
    }
}
