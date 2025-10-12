using HarmonyLib;

namespace IShowSeed.Random;

[TogglablePatch]
[HarmonyPatch(typeof(M_Level), "Awake")]
public static class M_Level_Awake_Patcher
{
    public static void Prefix(M_Level __instance)
    {
        __instance.canFlip = false;
    }
}
