using HarmonyLib;
using IShowSeed.Random;
using UnityEngine;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(UT_GameStateController), "RestartScene")]
public static class UT_GameStateController_Start_Patcher
{
    public static void Prefix()
    {
        Rod.Reset();
    }
}
