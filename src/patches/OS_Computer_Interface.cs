using System;
using System.Collections.Generic;
using System.Threading;
using HarmonyLib;

namespace IShowSeed.Patches;



[HarmonyPatch(typeof(OS_Computer_Interface), "Start")]
public static class OS_Computer_Interface_Start_Patcher
{
    public static readonly AccessTools.FieldRef<OS_Computer_Interface, int> seedRef = AccessTools.FieldRefAccess<OS_Computer_Interface, int>("seed");

    public static int callNumber = 0;
    private static int GetScopedSeed()
    {
        return IShowSeedPlugin.StartingSeed + Interlocked.Increment(ref callNumber);
    }

    [HarmonyPostfix]
    static void Postfix(OS_Computer_Interface __instance)
    {
        IShowSeedPlugin.mutex.WaitOne();

        seedRef(__instance) = GetScopedSeed();

        IShowSeedPlugin.mutex.ReleaseMutex();
    }
}


// seed will be set by the mod, not by the game
[HarmonyPatch(typeof(OS_Computer_Interface), "SetSeed")]
public static class OS_Computer_Interface_SetSeed_Patcher
{
    [HarmonyPrefix]
    static bool Prefix(OS_Computer_Interface __instance)
    {
        return false;
    }
}

