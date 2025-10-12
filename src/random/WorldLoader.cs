using HarmonyLib;

namespace IShowSeed.Random;

[TogglablePatch]
[HarmonyPatch(typeof(WorldLoader), "Awake")]
public static class WorldLoader_Awake_Patcher
{
    public static void Prefix()
    {
        WorldLoader.SetPresetSeed(IShowSeedPlugin.ConfigPresetSeed.Value.ToString());
        IShowSeedPlugin.Beep.LogInfo($"custom preset seed: {IShowSeedPlugin.ConfigPresetSeed.Value}");
        IShowSeedPlugin.StartingSeed = IShowSeedPlugin.ConfigPresetSeed.Value;
    }
}


[TogglablePatch]
[HarmonyPatch(typeof(WorldLoader), "Initialize")]
public static class WorldLoader_Initialize_Patcher
{
    public static void Prefix()
    {
        IShowSeedPlugin.Beep.LogInfo($"World initializing with seed: {IShowSeedPlugin.ConfigPresetSeed.Value}, resetting random...");
    }
}
