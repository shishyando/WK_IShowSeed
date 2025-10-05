using HarmonyLib;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(WorldLoader), "Awake")]
public static class WorldLoader_Awake_Patcher
{
    [HarmonyPrefix]
    public static void Prefix(WorldLoader __instance)
    {
        WorldLoader.SetPresetSeed(IShowSeedPlugin.configPresetSeed.Value.ToString());
        IShowSeedPlugin.Beep.LogInfo($"custom preset seed: {IShowSeedPlugin.configPresetSeed.Value}");
    }
}


[HarmonyPatch(typeof(WorldLoader), "Initialize")]
public static class WorldLoader_Initialize_Patcher
{
    [HarmonyPostfix]
    public static void Postfix(WorldLoader __instance)
    {
        IShowSeedPlugin.Beep.LogInfo($"starting seed: {__instance.startingSeed}");
        IShowSeedPlugin.StartingSeed = __instance.startingSeed;

        ENV_ArtifactDevice_Start_Patcher.callNumber = 0;
        SpawnSettings_RandomCheck_Patcher.callNumber = 0;
        ENV_VendingMachine_GenerateOptions_Patcher.callNumber = 0;
        OS_Computer_Interface_Start_Patcher.callNumber = 0;
    }
}
