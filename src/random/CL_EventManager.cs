using System.Collections.Generic;
using HarmonyLib;

namespace IShowSeed.Random;


[TogglablePatch]
[HarmonyPatch(typeof(CL_EventManager), "GetPossibleEvents")]
public static class SpawnSettings_GetPossibleEvents_Patcher
{
    [HarmonyPostfix]
    public static void Postfix(ref List<SessionEvent> __result)
    {
        __result?.RemoveAll(x => { return x.startCheck == SessionEvent.EventStart.checkEverySecond; });
    }

}

