using HarmonyLib;

namespace IShowSeed.Patches.Random;

[HarmonyPatch(typeof(WorldLoader), "Awake")]
public static class WorldLoader_Awake_Patcher
{
    [HarmonyPrefix]
    public static void StoreStartingSeed(WorldLoader __instance)
    {
        WorldLoader.SetPresetSeed(IShowSeedPlugin.configPresetSeed.Value.ToString());
        IShowSeedPlugin.Logger.LogInfo($"custom preset seed: {IShowSeedPlugin.configPresetSeed.Value}");
    }
}


[HarmonyPatch(typeof(WorldLoader), "Initialize")]
public static class WorldLoader_Initialize_Patcher
{
    [HarmonyPostfix]
    public static void RememberStartingSeed(WorldLoader __instance)
    {
        IShowSeedPlugin.Logger.LogInfo($"starting seed: {__instance.startingSeed}");
        IShowSeedPlugin.StartingSeed = __instance.startingSeed;

        UT_SpawnChance_Start_Patcher.callNumber = 1;
    }
}
