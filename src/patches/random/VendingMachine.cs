using System;
using System.Collections.Generic;
using System.Threading;
using HarmonyLib;

namespace IShowSeed.Patches.Random;

[HarmonyPatch(typeof(ENV_VendingMachine), "SetSeed")]
public static class ENV_VendingMachine_SetSeed_Patcher
{
    static readonly Action<ENV_VendingMachine> CallRegen = AccessTools.MethodDelegate<Action<ENV_VendingMachine>>(
        AccessTools.Method(typeof(ENV_VendingMachine), "RegenerateOptions", Type.EmptyTypes)
    );

    [ThreadStatic]
    private static Stack<UnityEngine.Random.State> stateStack;
    public static int callNumber = 1;
    private static int GetScopedSeed()
    {
        return IShowSeedPlugin.StartingSeed + Interlocked.Increment(ref callNumber);
    }

    [HarmonyPrefix]
    static void Prefix(ENV_VendingMachine __instance)
    {
        stateStack ??= new Stack<UnityEngine.Random.State>();
        var saved = UnityEngine.Random.state;
        stateStack.Push(saved);
        UnityEngine.Random.InitState(GetScopedSeed());

        int threadId = Thread.CurrentThread.ManagedThreadId;
        int objHash = __instance.GetHashCode();
        IShowSeedPlugin.Beep.LogInfo($"[PUSH STATE] ENV_VendingMachine prefix: thr={threadId}, obj={objHash}, call={callNumber}, state={UnityEngine.JsonUtility.ToJson(saved)}");
    }

    [HarmonyPostfix]
    public static void Postfix(ENV_VendingMachine __instance)
    {
        CallRegen(__instance);
        RestoreStateIfAny(__instance);
    }

    private static void RestoreStateIfAny(ENV_VendingMachine __instance)
    {
        if (stateStack != null && stateStack.Count > 0)
        {
            var prev = stateStack.Pop();
            UnityEngine.Random.state = prev;

            int threadId = Thread.CurrentThread.ManagedThreadId;
            int objHash = __instance.GetHashCode();
            IShowSeedPlugin.Beep.LogInfo($"[RESTORE STATE] ENV_VendingMachine prefix: thr={threadId}, obj={objHash}, call={callNumber}, state={UnityEngine.JsonUtility.ToJson(prev)}");
        }
    }
}

