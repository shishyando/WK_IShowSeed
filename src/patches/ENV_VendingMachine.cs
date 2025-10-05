using System;
using System.Collections.Generic;
using System.Threading;
using HarmonyLib;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(ENV_VendingMachine), "GenerateOptions")]
public static class ENV_VendingMachine_GenerateOptions_Patcher
{
    public static readonly AccessTools.FieldRef<ENV_VendingMachine, int> localSeedRef = AccessTools.FieldRefAccess<ENV_VendingMachine, int>("localSeed");

    private static Stack<UnityEngine.Random.State> stateStack;
    public static int callNumber = 0;
    private static int GetScopedSeed()
    {
        return IShowSeedPlugin.StartingSeed + Interlocked.Increment(ref callNumber);
    }

    [HarmonyPrefix]
    static void Prefix(ENV_VendingMachine __instance)
    {
        IShowSeedPlugin.mutex.WaitOne();
        stateStack ??= new Stack<UnityEngine.Random.State>();
        var saved = UnityEngine.Random.state;
        stateStack.Push(saved);
        localSeedRef(__instance) = GetScopedSeed();
        UnityEngine.Random.InitState(localSeedRef(__instance));
    }

    [HarmonyPostfix]
    public static void Postfix(ENV_VendingMachine __instance)
    {
        RestoreStateIfAny(__instance);
        IShowSeedPlugin.mutex.ReleaseMutex();
    }

    private static void RestoreStateIfAny(ENV_VendingMachine __instance)
    {
        if (stateStack != null && stateStack.Count > 0)
        {
            var prev = stateStack.Pop();
            UnityEngine.Random.state = prev;
        }
    }
}


// seed will be set by the mod, not by the game
[HarmonyPatch(typeof(ENV_VendingMachine), "SetSeed")]
public static class ENV_VendingMachine_SetSeed_Patcher
{
    [HarmonyPrefix]
    static bool Prefix(ENV_VendingMachine __instance)
    {
        return false;
    }
}

