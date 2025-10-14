using HarmonyLib;
using UnityEngine;

namespace IShowSeed.Random;

[TogglablePatch]
[HarmonyPatch(typeof(App_PerkPage), "GenerateCards", [typeof(bool)])]
public static class App_PerkPage_GenerateCards_Patcher
{
    private static readonly AccessTools.FieldRef<App_PerkPage, OS_Manager> osRef = AccessTools.FieldRefAccess<OS_Manager>(typeof(App_PerkPage), "os");
    private static readonly AccessTools.FieldRef<OS_Computer_Interface, int> seedRef = AccessTools.FieldRefAccess<int>(typeof(OS_Computer_Interface), "seed");

    public static void Prefix(ref Rod.Context __state, App_PerkPage __instance, bool refresh)
    {
        // get seed, save state, Random.InitState, restore later
        Rod.Enter(ref __state, GenerateCustomCallSite(__instance, refresh));
        // use direct access instead of patched out `SetSeed`
        seedRef(osRef(__instance).worldInterface) = -1; // so it does not call Random.InitState inside
    }

    public static void Finalizer(ref Rod.Context __state, App_PerkPage __instance)
    {
        seedRef(osRef(__instance).worldInterface) = (int)(__state.BaseSeed + __state.CallNumber); // for WorldDumper
        Rod.Exit(in __state);
    }

    private static string GenerateCustomCallSite(App_PerkPage i, bool refresh)
    {
        Transform tr = osRef(i).worldInterface.transform;
        return $"perkpage_{i.perkPageType}_{Helpers.LevelOf(tr)}_{i.minCards}_{i.maxCards}_{refresh}";
    }

}

