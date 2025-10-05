using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using HarmonyLib;
using UnityEngine;

namespace IShowSeed.Patches;


[HarmonyPatch(typeof(CL_EventManager), "GetPossibleEvents")]
public static class SpawnSettings_GetPossibleEvents_Patcher
{
    [HarmonyPostfix]
    static void Postfix(ref List<SessionEvent> __result, CL_EventManager __instance)
    {
        __result?.RemoveAll(x => { return x.startCheck == SessionEvent.EventStart.checkEverySecond; });
    }

}

