using System;
using System.Collections.Generic;
using System.Threading;
using HarmonyLib;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(App_PerkPage), "GenerateCards")]
public static class App_PerkPage_GenerateCards_Patcher
{
    private static Stack<UnityEngine.Random.State> stateStack;

    [HarmonyPrefix]
    static void Prefix(App_PerkPage __instance)
    {
        IShowSeedPlugin.mutex.WaitOne();
        stateStack ??= new Stack<UnityEngine.Random.State>();
        var saved = UnityEngine.Random.state;
        stateStack.Push(saved);
        // Random.InitState(os.WorldInterface.GetSeed()) is called inside
    }

    [HarmonyPostfix]
    public static void Postfix(App_PerkPage __instance)
    {
        RestoreStateIfAny(__instance);
        IShowSeedPlugin.mutex.ReleaseMutex();
    }

    private static void RestoreStateIfAny(App_PerkPage __instance)
    {
        if (stateStack != null && stateStack.Count > 0)
        {
            var prev = stateStack.Pop();
            UnityEngine.Random.state = prev;
        }
    }
}

