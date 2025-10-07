using HarmonyLib;
using IShowSeed.Random;

namespace IShowSeed.Patches;

[HarmonyPatch(typeof(ENV_VendingMachine), "GenerateOptions")]
public static class ENV_VendingMachine_GenerateOptions_Patcher
{
    private static readonly AccessTools.FieldRef<ENV_VendingMachine, int> localSeedRef = AccessTools.FieldRefAccess<ENV_VendingMachine, int>("localSeed");

    public static void Prefix(ref Rod.Context __state, ENV_VendingMachine __instance)
    {
        // get seed, save state, Random.InitState, restore later
        Rod.Enter(ref __state);
        localSeedRef(__instance) = __state.BaseSeed; // for WorldDumper
    }

    public static void Postfix(ref Rod.Context __state)
    {
        Rod.Exit(in __state);
    }
}


// seed will be set by the mod, not by the game
[HarmonyPatch(typeof(ENV_VendingMachine), "SetSeed")]
public static class ENV_VendingMachine_SetSeed_Patcher
{
    public static bool Prefix() => false;
}

