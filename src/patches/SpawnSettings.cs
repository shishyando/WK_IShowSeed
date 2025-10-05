using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HarmonyLib;
using UnityEngine;

namespace IShowSeed.Patches;

// basically determines all the random stuff except for perks, vendos and artifacts,
// it should be almost the only one to override
[HarmonyPatch(typeof(SpawnTable.SpawnSettings), "RandomCheck")]
public static class SpawnSettings_RandomCheck_Patcher
{
    private static Stack<UnityEngine.Random.State> stateStack;
    public static int callNumber = 0;
    private static int GetScopedSeed()
    {
        return IShowSeedPlugin.StartingSeed + Interlocked.Increment(ref callNumber);
    }

    [HarmonyPrefix]
    static void Prefix(SpawnTable.SpawnSettings __instance)
    {
        IShowSeedPlugin.mutex.WaitOne();
        stateStack ??= new Stack<UnityEngine.Random.State>();
        var saved = UnityEngine.Random.state;
        stateStack.Push(saved);
        UnityEngine.Random.InitState(GetScopedSeed());

        // logging
        int threadId = Thread.CurrentThread.ManagedThreadId;
        int objHash = __instance.GetHashCode();
        var st = new StackTrace(true);
        string traceStr = st.GetFrames().Select(f =>
        {
            var m = f.GetMethod();
            var dt = m.DeclaringType;
            if (dt == null) return "";
            return $" ==> {dt.FullName}.{m.Name}";
        }).Join(delimiter: "");
        IShowSeedPlugin.Beep.LogInfo($"[PSH] {__instance.GetType().FullName}: thr={threadId}, obj={objHash}, call={callNumber}, state={JsonUtility.ToJson(saved)}\n\tStackTrace {traceStr}\n===============");
    }

    [HarmonyPostfix]
    public static void Postfix(SpawnTable.SpawnSettings __instance)
    {
        RestoreStateIfAny(__instance);

        // logging
        int threadId = Thread.CurrentThread.ManagedThreadId;
        int objHash = __instance.GetHashCode();
        var st = new StackTrace(true);
        string traceStr = st.GetFrames().Select(f =>
        {
            var m = f.GetMethod();
            var dt = m.DeclaringType;
            if (dt == null) return "";
            return $" ==> {dt.FullName}.{m.Name}";
        }).Join(delimiter: "");
        IShowSeedPlugin.Beep.LogInfo($"[POP] {__instance.GetType().FullName}: thr={threadId}, obj={objHash}, call={callNumber}, state={JsonUtility.ToJson(UnityEngine.Random.state)}\n\tStackTrace {traceStr}\n===============");
        IShowSeedPlugin.mutex.ReleaseMutex();
    }

    private static void RestoreStateIfAny(SpawnTable.SpawnSettings __instance)
    {
        if (stateStack != null && stateStack.Count > 0)
        {
            var prev = stateStack.Pop();
            UnityEngine.Random.state = prev;
        }
    }

}

