using System;
using System.Collections.Generic;
using System.Threading;
using HarmonyLib;
using UnityEngine;

namespace IShowSeed.Patches.Random;


// Should seamlessly inject a custom RandomState (startingSeed + callNumber)
// then restore it via a thread-local stack of random states

[HarmonyPatch(typeof(UT_SpawnChance), "Start")]
public static class UT_SpawnChance_Start_Patcher
{
    [ThreadStatic]
    private static Stack<UnityEngine.Random.State> stateStack;
    // Main Menu calls this thing for background leves
    // so make sure to restore it in the WorldLoader.Initialize
    public static int callNumber = 1;
    private static int GetScopedSeed()
    {
        return IShowSeedPlugin.StartingSeed + Interlocked.Increment(ref callNumber);
    }

    [HarmonyPrefix]
    static void Prefix(UT_SpawnChance __instance)
    {
        stateStack ??= new Stack<UnityEngine.Random.State>();
        var saved = UnityEngine.Random.state;
        stateStack.Push(saved);
        UnityEngine.Random.InitState(GetScopedSeed());

        int threadId = Thread.CurrentThread.ManagedThreadId;
        int objHash = __instance.GetHashCode();
        IShowSeedPlugin.Beep.LogInfo($"[PUSH STATE] UT_SpawnChance prefix: thr={threadId}, obj={objHash}, call={callNumber}, state={JsonUtility.ToJson(saved)}");
    }


    [HarmonyPostfix]
    public static void Postfix(UT_SpawnChance __instance)
    {
        RestoreStateIfAny(__instance);
    }

    private static void RestoreStateIfAny(UT_SpawnChance __instance)
    {
        if (stateStack != null && stateStack.Count > 0)
        {
            var prev = stateStack.Pop();
            UnityEngine.Random.state = prev;

            int threadId = Thread.CurrentThread.ManagedThreadId;
            int objHash = __instance.GetHashCode();
            IShowSeedPlugin.Beep.LogInfo($"[RESTORE STATE] UT_SpawnChance prefix: thr={threadId}, obj={objHash}, call={callNumber}, state={JsonUtility.ToJson(prev)}");
        }
    }

}

