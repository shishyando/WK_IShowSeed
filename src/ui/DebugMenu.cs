using HarmonyLib;

namespace IShowSeed.Patches.UI;

[HarmonyPatch(typeof(CL_UIManager), "Update")]
public static class CL_UIManager_Update_Patcher
{
    public static void Postfix()
    {
        DebugMenu.UpdateDebugText("starting-seed", $"<color=blue>Starting seed: {IShowSeedPlugin.StartingSeed}</color>");
    }
}
