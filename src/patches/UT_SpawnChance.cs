using HarmonyLib;
using IShowSeed.Random;
using UnityEngine;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(UT_SpawnChance), "Start")]
public static class UT_SpawnChance_Start_Patcher
{
    public static void Prefix(UT_SpawnChance __instance)
    {
        // IShowSeedPlugin.Beep.LogInfo($"UT_SpawnChance start: {GetPath(__instance.gameObject.transform)}");
    }

    private static string GetPath(Transform t)
    {
        System.Text.StringBuilder sb = new(t.name);
        for (Transform p = t.parent; p != null; p = p.parent)
            sb.Insert(0, p.name + "/");
        return sb.ToString();
    }
}
 
