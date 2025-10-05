using HarmonyLib;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(M_Level), "Awake")]
public static class M_Level_Awake_Patcher
{
    public static void Prefix(M_Level __instance)
    {
        __instance.canFlip = false;
    }
}
