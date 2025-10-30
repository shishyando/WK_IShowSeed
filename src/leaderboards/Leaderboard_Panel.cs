
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using Steamworks;
using UnityEngine;

namespace IShowSeed
{
    [PermanentPatch]
    [HarmonyPatch(typeof(Leaderboard_Panel), "UpdateScore")]
    public static class SpawnSettings_UpdateScore_Patcher
    {
        public static bool FilterBySeed = false;

        public static bool Prefix(Leaderboard_Panel __instance)
        {
            if (Plugin.ConfigPresetSeed.Value == 0 || !MasterServer.Initialized)
            {
                return true;
            }

            try
            {
                // XXX: add toggle to filter by seed
                _ = OverrideLeaderboardsAsync(__instance);
            }
            catch (Exception e)
            {
                Plugin.Beep.LogError("An error occurred when trying to connect to leaderboards server! Make sure you have the latest version of the mod and default URI setting in mod's config");
                Plugin.Beep.LogError(e);
            }
            return false;
        }

        private static readonly AccessTools.FieldRef<Leaderboard_Panel, List<Leaderboard_Score>> scoreObjectsRef = AccessTools.FieldRefAccess<List<Leaderboard_Score>>(typeof(Leaderboard_Panel), "scoreObjects");

        public static async Task OverrideLeaderboardsAsync(Leaderboard_Panel __instance)
        {
            List<Leaderboard_Score> scoreObjects = scoreObjectsRef(__instance);
            string leaderboardsName = CL_GameManager.GetGamemodeName();

            for (int j = 0; j < scoreObjects.Count; j++)
            {
                scoreObjects[j].gameObject.SetActive(false);
            }
            List<MasterServer.LeaderboardEntry> lbScores = null;
            if (__instance.defaultType == Leaderboard_Panel.LeaderboardType.top)
            {
                lbScores = await MasterServer.FetchLeaderboards(leaderboardsName, __instance.scoreType, __instance.scoresToPull);
            }
            else if (__instance.defaultType == Leaderboard_Panel.LeaderboardType.friendsOnly || __instance.defaultType == Leaderboard_Panel.LeaderboardType.surrounding)
            {
                lbScores = await MasterServer.FetchLeaderboards(leaderboardsName, __instance.scoreType, __instance.scoresToPull, steamid: SteamClient.SteamId.Value.ToString());
                lbScores ??= await MasterServer.FetchLeaderboards(leaderboardsName, __instance.scoreType, __instance.scoresToPull);
            }

            for (int i = 0; i < scoreObjects.Count; i++)
            {
                if (i >= lbScores.Count)
                {
                    scoreObjects[i].gameObject.SetActive(false);
                    continue;
                }
                SteamId steamId = new() { Value = Convert.ToUInt64(lbScores[i].steamid) };
                Friend steamUser = new(steamId);
                scoreObjects[i].hmIcon.enabled = lbScores[i].hardmode;
                scoreObjects[i].ikIcon.enabled = lbScores[i].iron;
                scoreObjects[i].gameObject.SetActive(true);
                scoreObjects[i].positionText.text = $"<mspace=''>{lbScores[i].rank}</mspace>";
                scoreObjects[i].nameText.text = steamUser.Name ?? "";
                if (__instance.scoreType == Leaderboard_Panel.ScoreType.score)
                {
                    scoreObjects[i].scoreText.text = $"{lbScores[i].score}\n<color=grey>seed: {lbScores[i].seed}</color>";
                }
                else if (__instance.scoreType == Leaderboard_Panel.ScoreType.time)
                {
                    TimeSpan timeSpan = TimeSpan.FromSeconds(lbScores[i].time);
                    scoreObjects[i].scoreText.text = $"{timeSpan:hh\\:mm\\:ss} <color=grey>seed: {lbScores[i].seed}</color>";
                }
                else
                {
                    Plugin.Beep.LogWarning($"Unsupported scoreType {__instance.scoreType}");
                    return;
                }
                Texture2D texture = SteamManager.ConvertSteamIcon((await steamUser.GetMediumAvatarAsync()).Value);
                scoreObjects[i].profile.texture = texture;
                if (SteamClient.SteamId.Value == steamId.Value)
                {
                    scoreObjects[i].positionText.color = __instance.isMeColor;
                    scoreObjects[i].nameText.color = __instance.isMeColor;
                    scoreObjects[i].scoreText.color = __instance.isMeColor;
                }
                else
                {
                    scoreObjects[i].positionText.color = Color.white;
                    scoreObjects[i].nameText.color = Color.white;
                    scoreObjects[i].scoreText.color = Color.white;
                }
            }
        }

    }

}

