using HarmonyLib;

namespace IShowSeed.Random;

[OnlyForSeededRunsPatch]
[HarmonyPatch(typeof(UT_SPT_Spawner), "Spawn")]
public static class UT_SPT_Spawner_Spawn_Patcher
{
    public static void Prefix(ref Rod.Context __state, UT_SPT_Spawner __instance)
    {
        // get seed, save state, Random.InitState, restore later
        Rod.Enter(ref __state, GenerateCustomCallSite(__instance));
    }

    public static void Finalizer(ref Rod.Context __state)
    {
        Rod.Exit(in __state);
    }

    private static string GenerateCustomCallSite(UT_SPT_Spawner i)
    {
        return $"UT_SPT_{i.gameObject.name}";
    }
}

