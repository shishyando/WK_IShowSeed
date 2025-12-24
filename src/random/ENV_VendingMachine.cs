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

    private static string GenerateCustomCallSite(ENV_VendingMachine instance)
    {
        string callSite = $"vendo_{Helpers.LevelOfStr(instance.transform)}_{instance.gameObject.name}";
        return callSite;
    }
}

[OnlyForSeededRunsPatch]
[HarmonyPatch(typeof(ENV_VendingMachine), "Buy", [typeof(int), typeof(bool), typeof(bool)])]
public static class ENV_VendingMachine_Buy_Patcher
{
    private static readonly AccessTools.FieldRef<ENV_VendingMachine, bool> deadRef = AccessTools.FieldRefAccess<ENV_VendingMachine, bool>("dead");

    public static void Prefix(ref Rod.Context __state, ENV_VendingMachine __instance, ref int i, bool force, bool free)
    {
        if (!(force && free && deadRef(__instance)))
        {
            return;
        }
        // get seed, save state, Random.InitState, restore later
        Rod.Enter(ref __state, GenerateCustomCallSite(__instance));
        i = UnityEngine.Random.Range(0, __instance.buttons.Length);
    }

    public static void Finalizer(ref Rod.Context __state)
    {
        Rod.Exit(in __state);
    }

    private static string GenerateCustomCallSite(ENV_VendingMachine instance)
    {
        string callSite = $"vendo_{Helpers.LevelOfStr(instance.transform)}_{instance.gameObject.name}_dead";
        return callSite;
    }
}


// seed will be set by the mod, not by the game
[OnlyForSeededRunsPatch]
[HarmonyPatch(typeof(ENV_VendingMachine), "SetSeed")]
public static class ENV_VendingMachine_SetSeed_Patcher
{
    public static bool Prefix() => false;
}

