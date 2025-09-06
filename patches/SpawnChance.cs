using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HarmonyLib;
using UnityEngine;

namespace IShowSeed.Patches;


// Should seamlessly inject a custom RandomState (startingSeed + callNumber)
// then restore it via a thread-local stack of random states

[HarmonyPatch(typeof(UT_SpawnChance), "Start")]
public static class UT_SpawnChance_Start_Patcher
{
    static readonly AccessTools.FieldRef<UT_SpawnChance, float> chanceDebugRef = AccessTools.FieldRefAccess<UT_SpawnChance, float>("chanceDebug");

    [ThreadStatic]
    private static Stack<UnityEngine.Random.State> stateStack;

    // Main Menu calls this thing for background leves
    // so make sure to restore it in the WorldLoader.Initialize
    public static int callNumber = 1;

    private static int NextCallNumber()
    {
        return Interlocked.Increment(ref callNumber);
    }

    private static int GetScopedSeed()
    {
        int callNumber = NextCallNumber();
        return IShowSeedPlugin.StartingSeed + callNumber;
    }

    [HarmonyPrefix]
    static void Prefix()
    {
        if (stateStack == null) {
            stateStack = new Stack<UnityEngine.Random.State>();
        }

        var saved = UnityEngine.Random.state;
        stateStack.Push(saved);

        UnityEngine.Random.InitState(GetScopedSeed());
    }


    [HarmonyPostfix]
    public static void Postfix(UT_SpawnChance __instance)
    {
        RestoreStateIfAny();

        // Helpers.DebugHelper.LogTransform(__instance.transform, nameof(UT_SpawnChance));
    }

    private static void RestoreStateIfAny()
    {
        if (stateStack != null && stateStack.Count > 0)
        {
            var prev = stateStack.Pop();
            UnityEngine.Random.state = prev;
        }
    }

    static void Postfix()
    {
        RestoreStateIfAny();
    }

    static Exception Finalizer(Exception __exception)
    {
        RestoreStateIfAny();
        return __exception;
    }

}

