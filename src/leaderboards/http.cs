using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IShowSeed;
using Newtonsoft.Json;
using Steamworks;
using UnityEngine;

internal static class MasterServer
{
    internal static HttpClient HttpClient;
    internal static bool Initialized = false;

    public static async Task InitializeHttpClient()
    {
        HttpClient = new();
        HttpClient.Timeout = TimeSpan.FromSeconds(Plugin.TimeoutSeconds.Value);
        string[] uris = [
            Plugin.LeaderboardUri.Value,
            "http://128.199.54.23:80", // Amsterdam main server
            "http://158.160.65.211:5252", // Proxy in Russia
        ];

        foreach (string uri in uris)
        {
            try
            {
                Plugin.Beep.LogInfo($"Checking seeded runs leaderboard uri: {uri}");
                HttpClient.BaseAddress = new Uri(uri);
                HttpResponseMessage healthCheck = await HttpClient.GetAsync("/health");
                healthCheck.EnsureSuccessStatusCode();
                Initialized = true;
                break;
            }
            catch (Exception)
            {
                Plugin.Beep.LogInfo($"Uri `{uri}` failed");
            }
        }
        if (!Initialized) return;
        Plugin.Beep.LogInfo($"Connected to {HttpClient.BaseAddress}");
        Plugin.LeaderboardUri.Value = HttpClient.BaseAddress.ToString();
    }

    public static async Task<List<LeaderboardEntry>> FetchLeaderboards(string gamemode, Leaderboard_Panel.ScoreType scoreType, int limit = 10, string steamid = "")
    {
        if (!Initialized) return [];
        string endpoint = string.Format($"getstats?gamemode={gamemode}&limit={limit}&sortby={scoreType}&steamid={steamid}");
        Plugin.Beep.LogWarning($"Fetching Leaderboards: {endpoint}");

        using HttpResponseMessage response = await HttpClient.GetAsync(endpoint);
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
        if (!Initialized) return;
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
        using HttpResponseMessage response = await HttpClient.PostAsync("uploadscore", jsonContent);
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
