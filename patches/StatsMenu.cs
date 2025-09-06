using HarmonyLib;
using TMPro;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(UT_SeededEnable), "OnEnable")]
public static class UT_SeededEnable_OnEnable_Patcher
{
    [HarmonyPrefix]
    public static bool ShowSeedText(UT_SeededEnable __instance)
    {
        TextMeshProUGUI text = __instance.gameObject.GetComponent<TextMeshProUGUI>();
        text.text = $"SEED: {IShowSeedPlugin.StartingSeed}";
        if (WorldLoader.customSeed) {
            text.text = "PRESET " + text.text;
        }
        return false; // skips OnEnable which calls OnCheckStatus to hide the text when WorldLoader.customSeed = false
    }
}

