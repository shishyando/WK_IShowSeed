using HarmonyLib;

namespace IShowSeed.Random;


[TogglablePatch]
[HarmonyPatch(typeof(ENV_ArtifactDevice), "Start")]
public static class ENV_ArtifactDevice_Start_Patcher
{
    public static void Prefix(ref Rod.Context __state)
    {
        // get seed, save state, Random.InitState, restore later
        Rod.Enter(ref __state);
    }

    public static void Finalizer(ref Rod.Context __state)
    {
        Rod.Exit(in __state);
    }

}

