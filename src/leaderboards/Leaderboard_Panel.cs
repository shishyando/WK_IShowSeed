using System.Collections.Generic;
using HarmonyLib;
using Steamworks;

namespace IShowSeed.Random;


[TogglablePatch]
[HarmonyPatch(typeof(Leaderboard_Panel), "UpdateScore")]
public static class SpawnSettings_UpdateScore_Patcher
{
    public static bool FilterBySeed = false;

    [HarmonyPostfix]
    public static void Postfix(Leaderboard_Panel __instance)
    {
        if (Plugin.ConfigPresetSeed.Value == 0) {
            return true;
        }
        // XXX: add toggle to filter by seed
        Plugin.Beep.LogInfo("fetching custom leaderboards for seeded runs");

        OverrideLeaderboardsAsync(__instance);
    }

    public static async Task OverrideLeaderboardsAsync(Leaderboard_Panel __instance) {
        List<Leaderboard_Score> scoreObjects = __instance.scoreObjects;
        for (int j = 0; j < scoreObjects.Count; j++)
        {
            ((Component)scoreObjects[j]).gameObject.SetActive(false);
        }
        bool useTime = (int)__instance.scoreType == 1;
        List<LeaderboardEntry> lbScores = null;
        if ((int)__instance.defaultType == 0)
        {
            lbScores = await FetchLeaderboards(CurrentSeed.id, (Object)(object)CL_GameManager.gamemode == (Object)(object)dailyMode, __instance.scoresToPull, useTime);
        }
        else if ((int)__instance.defaultType == 2 || (int)__instance.defaultType == 1)
        {
            lbScores = await FetchNearbyLeaderboards(CurrentSeed.id, (Object)(object)CL_GameManager.gamemode == (Object)(object)dailyMode, __instance.scoresToPull / 2, SteamClient.SteamId.Value.ToString(), useTime);
            if (lbScores == null)
            {
                lbScores = await FetchLeaderboards(CurrentSeed.id, (Object)(object)CL_GameManager.gamemode == (Object)(object)dailyMode, __instance.scoresToPull, useTime);
            }
        }
        Friend val = default(Friend);
        for (int i = 0; i < scoreObjects.Count; i++)
        {
            if (i >= lbScores.Count)
            {
                ((Component)scoreObjects[i]).gameObject.SetActive(false);
                continue;
            }
            SteamId id = new SteamId
            {
                Value = Convert.ToUInt64(lbScores[i].steamid)
            };
            ((Friend)(ref val))..ctor(id);
            ((Component)scoreObjects[i]).gameObject.SetActive(true);
            scoreObjects[i].positionText.text = $"<mspace=''>{lbScores[i].rank}</mspace>";
            scoreObjects[i].nameText.text = ((Friend)(ref val)).Name ?? "";
            if (!useTime)
            {
                scoreObjects[i].scoreText.text = $"{lbScores[i].score} <color=grey>seed: {lbScores[i].seed}</color>";
            }
            else
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds(lbScores[i].time.Value);
                scoreObjects[i].scoreText.text = $"{timeSpan:hh\\:mm\\:ss} <color=grey>seed: {lbScores[i].seed}</color>";
                // XXX: add milis
            }
            Texture2D texture = SteamManager.ConvertSteamIcon((await ((Friend)(ref val)).GetMediumAvatarAsync()).Value);
            scoreObjects[i].profile.texture = (Texture)(object)texture;
            if (SteamClient.SteamId.Value == id.Value)
            {
                ((Graphic)scoreObjects[i].positionText).color = __instance.isMeColor;
                ((Graphic)scoreObjects[i].nameText).color = __instance.isMeColor;
                ((Graphic)scoreObjects[i].scoreText).color = __instance.isMeColor;
            }
            else
            {
                ((Graphic)scoreObjects[i].positionText).color = Color.white;
                ((Graphic)scoreObjects[i].nameText).color = Color.white;
                ((Graphic)scoreObjects[i].scoreText).color = Color.white;
            }
        }
    }

    public static async Task<List<LeaderboardEntry>> FetchLeaderboards(int seedId, bool daily, int count = 10, bool time = false)
    {
        using HttpResponseMessage response = await Plugin.HttpClient.GetAsync(string.Format("statsforseed?leaderboard={0}{1}&count={2}{3}", "gamemode", FilterBySeed ? $"&seed={seedId}" : "", count, time ? "&sorttime=1" : ""));
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<List<LeaderboardEntry>>(await response.Content.ReadAsStringAsync());
    }
    public static async Task<List<LeaderboardEntry>> FetchNearbyLeaderboards(int seedId, bool daily, int count = 10, string steamId = "", bool time = false)
    {
        using HttpResponseMessage response = await Plugin.HttpClient.GetAsync(string.Format("statsnearplayer?leaderboard={0}{1}&count={2}{3}&steamId={4}", "gamemode", FilterBySeed ? $"&seed={seedId}" : "", count, time ? "&sorttime=1" : "", steamId));
        response.EnsureSuccessStatusCode();
        return JsonConvert.DeserializeObject<List<LeaderboardEntry>>(await response.Content.ReadAsStringAsync());
    }

    public static async void UploadScore(string gamemode, int seed, float score = 0f, float time = 0f)
    {
        if ((Object)(object)CL_GameManager.gamemode == (Object)(object)dailyMode && HasCompetedInDaily)
        {
            return;
        }
        using StringContent jsonContent = new StringContent(JsonConvert.SerializeObject((object)new Dictionary<string, object>
        {
            {
                "steamid",
                SteamClient.SteamId.Value.ToString()
            },
            {
                "score",
                Mathf.FloorToInt(Mathf.Clamp(score, 0f, 2.1474836E+09f))
                // XXX: times 1000 ?
            },
            {
                "seed",
                seed,
            },
            {
                "gamemode",
                gamemode
            },
            {
                "time",
                Mathf.FloorToInt(Mathf.Clamp(time, 0f, 2.1474836E+09f))
                // XXX: times 1000 ?
            }
        }, (Formatting)0), Encoding.UTF8, "application/json");
        HttpResponseMessage obj = await Plugin.http.PostAsync("sign", jsonContent);
        obj.EnsureSuccessStatusCode();
        string content = await obj.Content.ReadAsStringAsync();
        (await Plugin.http.PostAsync("uploadscore", new StringContent(content))).EnsureSuccessStatusCode();
    }

}

public record LeaderboardEntry(string steamid = null, int? score = null, int? rank = null, int? time = null, int? seed = null);
