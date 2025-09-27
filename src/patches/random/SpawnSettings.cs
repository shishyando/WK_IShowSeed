using System;
using System.Collections.Generic;
using System.Threading;
using HarmonyLib;
using UnityEngine;

namespace IShowSeed.Patches.Random;

[HarmonyPatch(typeof(SpawnTable.SpawnSettings), "RandomCheck")]
public static class SpawnSettings_RandomCheck_Patcher
{
    [ThreadStatic]
    private static Stack<UnityEngine.Random.State> stateStack;
    public static int callNumber = 1;
    private static int GetScopedSeed()
    {
        return IShowSeedPlugin.StartingSeed + Interlocked.Increment(ref callNumber);
    }

    [HarmonyPrefix]
    static void Prefix(SpawnTable.SpawnSettings __instance)
    {

        stateStack ??= new Stack<UnityEngine.Random.State>();
        var saved = UnityEngine.Random.state;
        stateStack.Push(saved);
        UnityEngine.Random.InitState(GetScopedSeed());

        int threadId = Thread.CurrentThread.ManagedThreadId;
        int objHash = __instance.GetHashCode();
        IShowSeedPlugin.Beep.LogInfo($"[PUSH STATE] SpawnTable.SpawnSettings prefix: thr={threadId}, obj={objHash}, call={callNumber}, state={JsonUtility.ToJson(saved)}");
    }


    [HarmonyPostfix]
    public static void Postfix(SpawnTable.SpawnSettings __instance)
    {
        RestoreStateIfAny(__instance);
    }

    private static void RestoreStateIfAny(SpawnTable.SpawnSettings __instance)
    {
        if (stateStack != null && stateStack.Count > 0)
        {
            var prev = stateStack.Pop();
            UnityEngine.Random.state = prev;

            int threadId = Thread.CurrentThread.ManagedThreadId;
            int objHash = __instance.GetHashCode();
            IShowSeedPlugin.Beep.LogInfo($"[RESTORE STATE] SpawnTable.SpawnSettings postfix: thr={threadId}, obj={objHash}, call={callNumber}, state={JsonUtility.ToJson(prev)}");
        }
    }

}

