using HarmonyLib;

namespace IShowSeed.Random;

[TogglablePatch]
[HarmonyPatch(typeof(WorldLoader), "Awake")]
public static class WorldLoader_Awake_Patcher
{
    public static void Prefix()
    {
        WorldLoader.SetPresetSeed(Plugin.ConfigPresetSeed.Value.ToString());
        Plugin.Beep.LogInfo($"custom preset seed: {Plugin.ConfigPresetSeed.Value}");
        Plugin.SeedForRandom = Plugin.ConfigPresetSeed.Value;
    }
}


[TogglablePatch]
[HarmonyPatch(typeof(WorldLoader), "Initialize")]
public static class WorldLoader_Initialize_Patcher
{
    public static void Prefix()
    {
        Plugin.Beep.LogInfo($"World initializing with seed: {Plugin.ConfigPresetSeed.Value}, resetting random...");
    }
}
