using HarmonyLib;

namespace IShowSeed;

[PermanentPatch]
[HarmonyPatch(typeof(M_Gamemode), "Finish")]
public static class M_Gamemode_Finish_Patcher
{
    public static bool Prefix(M_Gamemode __instance, float time, bool hasFinished)
    {
        if (Plugin.IsRandomRun())
        {
            return true;
        }
        __instance.gamemodeModule.OnFinish(hasFinished);
        bool banned = (!__instance.allowCheatedScores && CommandConsole.hasCheated) || (!__instance.allowCheatedScores && !CL_GameManager.AreAchievementsAllowed());
        try
        {
            MasterServer.UploadScore(
                __instance.GetGamemodeName(),
                Plugin.SeedForRandom,
                banned ? 0f : __instance.GetPlayerScore(hasFinished),
                time,
                __instance.IsIronKnuckle(),
                __instance.allowHardmode && CL_GameManager.IsHardmode(),
                hasFinished
            );
        }
        catch (System.Exception e)
        {
            Plugin.Beep.LogError("An error occurred when trying to connect to leaderboards server! Make sure you have the latest version of the mod and default URI setting in mod's config");
            Plugin.Beep.LogError(e);
        }
        return false;
    }

}

