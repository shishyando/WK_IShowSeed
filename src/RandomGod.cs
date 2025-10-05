using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace IShowSeed.Random;

internal static class RandomGod
{
    private static readonly Mutex _mutex = new();
    private static readonly Dictionary<string, UnityEngine.Random.State> _stateBySite = [];

    public struct Context
    {
        internal UnityEngine.Random.State PrevGlobalState;
        internal string SiteKey;
    }

    public static void Enter(ref Context ctx)
    {
        ctx = default;
        _mutex.WaitOne();
        ctx.PrevGlobalState = UnityEngine.Random.state;

        string siteKey = ComputeSiteKey();
        ctx.SiteKey = siteKey;

        if (!_stateBySite.TryGetValue(siteKey, out var siteState))
        {
            var prev = UnityEngine.Random.state;
            int seed = DeriveSeed(IShowSeedPlugin.StartingSeed, siteKey);
            UnityEngine.Random.InitState(seed);
            siteState = UnityEngine.Random.state;
            UnityEngine.Random.state = prev;

            _stateBySite[siteKey] = siteState;
        }

        UnityEngine.Random.state = _stateBySite[siteKey];
    }

    public static void Exit(in Context ctx)
    {
        var newState = UnityEngine.Random.state;
        _stateBySite[ctx.SiteKey] = newState;
        UnityEngine.Random.state = ctx.PrevGlobalState;
        _mutex.ReleaseMutex(); 
    }

    private static string ComputeSiteKey()
    {
        var st = new StackTrace(3, false);
        var f = st.FrameCount > 0 ? st.GetFrame(0) : null;
        var mb = f?.GetMethod();

        return mb != null
            ? $"{mb.DeclaringType?.FullName}.{mb.Name}"
            : "UnknownCallSite";
    }

    private static int DeriveSeed(int masterSeed, string siteKey)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes($"{masterSeed}:{siteKey}");
        var hash = sha.ComputeHash(bytes);
        return BitConverter.ToInt32(hash, 0);
    }
}
