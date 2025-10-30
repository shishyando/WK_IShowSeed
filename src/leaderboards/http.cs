using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IShowSeed;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

public static class LeaderboardManager
{
    public static async Task<List<LeaderboardEntry>> FetchLeaderboards(string gamemode, Leaderboard_Panel.ScoreType scoreType, int limit = 10, string steamid = "")
    {
        string endpoint = string.Format($"getstats?gamemode={gamemode}&limit={limit}&sortby={scoreType}&steamid={steamid}");
        Plugin.Beep.LogWarning($"Fetching Leaderboards: {endpoint}");

        using HttpResponseMessage response = await Plugin.HttpClient.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
        {
            Plugin.Beep.LogError(response);
            response.EnsureSuccessStatusCode();
        }
        string res = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<LeaderboardEntry>>(res);
    }

    public static async void UploadScore(string gamemode, int seed, float score, float time, bool iron, bool hardmode, bool finished)
    {
        Plugin.Beep.LogWarning($"Uploading run info: gamemode={gamemode}, seed={seed}, score={score}, time={time}, iron={iron}, hardmode={hardmode}, finished={finished}");
        using StringContent jsonContent = new(JsonConvert.SerializeObject(new Dictionary<string, object>
        {
            { "gamemode", gamemode },
            { "steamid", SteamClient.SteamId.Value.ToString() },
            { "seed", seed },
            { "time", Mathf.FloorToInt(Mathf.Clamp(time, 0f, 2.1474836E+09f)) },
            { "score", Mathf.FloorToInt(Mathf.Clamp(score, 0f, 2.1474836E+09f)) },
            { "iron", iron },
            { "hardmode", hardmode },
            { "finished", finished }
        }, 0), Encoding.UTF8, "application/json");
        using HttpResponseMessage response = await Plugin.HttpClient.PostAsync("uploadscore", jsonContent);
        if (!response.IsSuccessStatusCode)
        {
            Plugin.Beep.LogError(response);
            response.EnsureSuccessStatusCode();
        }
    }

    public record LeaderboardEntry(string steamid, int rank, int score, int time, int seed, bool hardmode, bool iron, bool finished);
}


namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
