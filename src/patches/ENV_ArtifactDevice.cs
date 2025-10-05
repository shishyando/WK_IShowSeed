using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HarmonyLib;

namespace IShowSeed.Patches;


[HarmonyPatch(typeof(ENV_ArtifactDevice), "Start")]
public static class ENV_ArtifactDevice_Start_Patcher
{
    private static Stack<UnityEngine.Random.State> stateStack;
    public static int callNumber = 0;
    private static int GetScopedSeed()
    {
        return IShowSeedPlugin.StartingSeed + Interlocked.Increment(ref callNumber);
    }

    [HarmonyPrefix]
    static void Prefix(ENV_ArtifactDevice __instance)
    {
        IShowSeedPlugin.mutex.WaitOne();
        stateStack ??= new Stack<UnityEngine.Random.State>();
        var saved = UnityEngine.Random.state;
        stateStack.Push(saved);
        int seed = GetScopedSeed();
        UnityEngine.Random.InitState(seed);
        IShowSeedPlugin.Beep.LogInfo($"ENV_ArtifactDevice prefix called with seed: {seed}, {__instance.artifacts.Select(x => { return x.itemData.itemName;  }).Join()}");
    }


    [HarmonyPostfix]
    public static void Postfix(ENV_ArtifactDevice __instance)
    {
        RestoreStateIfAny(__instance);
        IShowSeedPlugin.mutex.ReleaseMutex();
    }

    private static void RestoreStateIfAny(ENV_ArtifactDevice __instance)
    {
        if (stateStack != null && stateStack.Count > 0)
        {
            var prev = stateStack.Pop();
            UnityEngine.Random.state = prev;
        }
    }

}

