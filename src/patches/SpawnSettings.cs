using HarmonyLib;
using IShowSeed.Random;

namespace IShowSeed.Patches;

// basically determines all the random stuff except for perks, vendos and artifacts,
// it should be almost the only one to override
[HarmonyPatch(typeof(SpawnTable.SpawnSettings), "RandomCheck")]
public static class SpawnSettings_RandomCheck_Patcher
{
    public static bool Prefix(ref Rod.Context __state, ref bool __result, SpawnTable.SpawnSettings __instance)
    {
        // first check predefined spawns (like refresh buttons and other stuff which may be annoying)
        float chance = __instance.GetEffectiveSpawnChance();
        if (chance == 0f)
        {
            __result = false;
            return false;
        }
        if (chance == 1f)
        {
            __result = true;
            return false;
        }

        // get seed, save state, Random.InitState, restore later
        Rod.Enter(ref __state);
        return true;
    }

    public static void Postfix(ref Rod.Context __state)
    {
        Rod.Exit(in __state);
    }

}

