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
        Plugin.Beep.LogWarning($"fetching leaderboards: {endpoint}");

        using HttpResponseMessage response = await Plugin.HttpClient.GetAsync(endpoint);
        if (!response.IsSuccessStatusCode)
        {
            Plugin.Beep.LogError(response);
            response.EnsureSuccessStatusCode();
        }
        string res = await response.Content.ReadAsStringAsync();
        Plugin.Beep.LogInfo($"OK: {res}");
        return JsonConvert.DeserializeObject<List<LeaderboardEntry>>(res);
    }

    public static async void UploadScore(string gamemode, int seed, float score, float time, bool iron, bool hardmode)
    {
        Plugin.Beep.LogWarning($"Upload score called, gamemode={gamemode}, seed={seed}, score={score}, time={time}, iron={iron}, hardmode={hardmode}");
        using StringContent jsonContent = new(JsonConvert.SerializeObject(new Dictionary<string, object>
        {
            { "gamemode", gamemode },
            { "steamid", SteamClient.SteamId.Value.ToString() },
            { "seed", seed },
            { "time", Mathf.FloorToInt(Mathf.Clamp(time, 0f, 2.1474836E+09f)) },
            { "score", Mathf.FloorToInt(Mathf.Clamp(score, 0f, 2.1474836E+09f)) },
            { "iron", iron },
            { "hardmode", hardmode }
        }, 0), Encoding.UTF8, "application/json");
        (await Plugin.HttpClient.PostAsync("uploadscore", jsonContent)).EnsureSuccessStatusCode();
        Plugin.Beep.LogInfo($"OK: {jsonContent}");
    }

    public record LeaderboardEntry(string steamid, int rank, int score, int time, int seed, bool hardmode, bool iron);
}


namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}
