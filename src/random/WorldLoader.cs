using HarmonyLib;

namespace IShowSeed.Random;

[OnlyForSeededRunsPatch]
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


[PermanentPatch]
[HarmonyPatch(typeof(WorldLoader), "Initialize")]
public static class WorldLoader_Initialize_Patcher
{
    public static void Postfix()
    {
        if (Plugin.IsRandomRun())
        {
            Plugin.SeedForRandom = WorldLoader.instance.seed;
        }
    }
}
