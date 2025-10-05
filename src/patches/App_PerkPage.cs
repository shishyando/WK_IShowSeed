using HarmonyLib;
using IShowSeed.Random;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(App_PerkPage), "GenerateCards")]
public static class App_PerkPage_GenerateCards_Patcher
{
    private static readonly AccessTools.FieldRef<App_PerkPage, OS_Manager> osRef = AccessTools.FieldRefAccess<OS_Manager>(typeof(App_PerkPage), "os");

    public static void Prefix(ref Rod.Context __state, App_PerkPage __instance)
    {
        // get seed, save state, Random.InitState, restore later
        Rod.Enter(ref __state);
        osRef(__instance).worldInterface.SetSeed(-1); // so it does not call Random.InitState inside
    }

    public static void Postfix(ref Rod.Context __state, App_PerkPage __instance)
    {
        osRef(__instance).worldInterface.SetSeed(__state.Seed);
        Rod.Exit(in __state);
    }

}

