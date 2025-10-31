using HarmonyLib;

namespace IShowSeed.Random;

[OnlyForSeededRunsPatch]
[HarmonyPatch(typeof(ENV_VendingMachine), "GenerateOptions")]
public static class ENV_VendingMachine_GenerateOptions_Patcher
{
    private static readonly AccessTools.FieldRef<ENV_VendingMachine, int> localSeedRef = AccessTools.FieldRefAccess<ENV_VendingMachine, int>("localSeed");

    public static void Prefix(ref Rod.Context __state, ENV_VendingMachine __instance)
    {
        // get seed, save state, Random.InitState, restore later
        Rod.Enter(ref __state, GenerateCustomCallSite(__instance));
        localSeedRef(__instance) = (int)(__state.BaseSeed + __state.CallNumber); // for WorldDumper
    }

    public static void Finalizer(ref Rod.Context __state)
    {
        Rod.Exit(in __state);
    }

    private static string GenerateCustomCallSite(ENV_VendingMachine i)
    {
        UnityEngine.Transform tr = i.transform;
        return $"vendo_{Helpers.LevelOf(tr)}_{tr.position.x:F2}_{tr.position.y:F2}_{tr.position.z:F2}";
    }
}


// seed will be set by the mod, not by the game
[OnlyForSeededRunsPatch]
[HarmonyPatch(typeof(ENV_VendingMachine), "SetSeed")]
public static class ENV_VendingMachine_SetSeed_Patcher
{
    public static bool Prefix() => false;
}

