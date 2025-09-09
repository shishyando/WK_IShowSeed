using System;
using HarmonyLib;

namespace IShowSeed.Patches.Random;

[HarmonyPatch(typeof(ENV_VendingMachine), "SetSeed")]
public static class ENV_VendingMachine_SetSeed_Patcher
{
    static readonly AccessTools.FieldRef<ENV_VendingMachine, int> SeedRef = AccessTools.FieldRefAccess<ENV_VendingMachine, int>("localSeed");
    static readonly Action<ENV_VendingMachine> CallRegen = AccessTools.MethodDelegate<Action<ENV_VendingMachine>>(
        AccessTools.Method(typeof(ENV_VendingMachine), "RegenerateOptions", Type.EmptyTypes)
    );

    [HarmonyPostfix]
    public static void SetRandomStateAndRegen(ENV_VendingMachine __instance)
    {
        // UnityEngine.Random.InitState(SeedRef(__instance));
        // CallRegen(__instance);
    }
}

