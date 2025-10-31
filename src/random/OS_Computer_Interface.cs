using HarmonyLib;

namespace IShowSeed.Random;


// seed will be set by the mod, not by the game
[OnlyForSeededRunsPatch]
[HarmonyPatch(typeof(OS_Computer_Interface), "SetSeed")]
public static class OS_Computer_Interface_SetSeed_Patcher
{
    public static bool Prefix() => false;
}
