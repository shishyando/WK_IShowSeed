using System.Collections.Generic;
using HarmonyLib;

namespace IShowSeed.Random;


[PermanentPatch]
[HarmonyPatch(typeof(CL_EventManager), "GetPossibleEvents")]
public static class CL_EventManager_GetPossibleEvents_Patcher
{
    public static void Postfix(ref List<SessionEvent> __result)
    {
        if (Plugin.IsSeededRun())
        {
            __result?.RemoveAll(x => { return x.startCheck == SessionEvent.EventStart.checkEverySecond; });
        }
    }

}

