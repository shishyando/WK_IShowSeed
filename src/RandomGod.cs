using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace IShowSeed.Random;

public static class Rod // short for RandomGod
{
    private static readonly object _lock = new();
    private static readonly Dictionary<string, UnityEngine.Random.State> _stateBySite = [];
    private static bool _enabled;

    public struct Context
    {
        public UnityEngine.Random.State PrevRandomState;
        public string SiteKey;
        public int Seed;
        public bool Valid;
    }

    internal static void Enter(ref Context ctx)
    {
        // IShowSeedPlugin.Beep.LogInfo($"ROD: Enter called");
        if (!_enabled) return;
        ctx = default;
        ctx.SiteKey = ComputeSiteKey();
        ctx.Seed = DeriveSeed(ctx.SiteKey);
        Monitor.Enter(_lock);
        ctx.PrevRandomState = UnityEngine.Random.state;

        if (!_stateBySite.ContainsKey(ctx.SiteKey))
        {
            // create a new Random.State which will advance in future
            // have to InitState with a derived seed to save it
            var tmpPrev = UnityEngine.Random.state;
            UnityEngine.Random.InitState(ctx.Seed);
            UnityEngine.Random.State siteState = UnityEngine.Random.state;
            UnityEngine.Random.state = tmpPrev;

            _stateBySite[ctx.SiteKey] = siteState;
        }

        UnityEngine.Random.state = _stateBySite[ctx.SiteKey];
        ctx.Valid = true;
        // IShowSeedPlugin.Beep.LogInfo($"ROD: Enter COMPLETED!, {ctx.SiteKey}");
    }

    internal static void Exit(in Context ctx)
    {
        // IShowSeedPlugin.Beep.LogInfo($"ROD: Exit called, ctx.Valid={ctx.Valid}");
        if (!_enabled || !ctx.Valid) return;
        var newState = UnityEngine.Random.state;
        if (ctx.SiteKey == null)
        {
            IShowSeedPlugin.Beep.LogError($"SiteKey is null, {GetStackTraceStr(0)}");
            return;
        }
        _stateBySite[ctx.SiteKey] = newState;
        UnityEngine.Random.state = ctx.PrevRandomState;
        Monitor.Exit(_lock);
        // IShowSeedPlugin.Beep.LogInfo($"ROD: Exit COMPLETED!");
    }

    private static string ComputeSiteKey()
    {
        string traceStr = GetStackTraceStr(2);
        // IShowSeedPlugin.Beep.LogInfo($"ROD: ComputeSiteKey is called");

        return traceStr;
    }

    private static int DeriveSeed(string siteKey)
    {
        // IShowSeedPlugin.Beep.LogInfo($"ROD: DeriveSeed called");
        return Animator.StringToHash(siteKey) ^ IShowSeedPlugin.StartingSeed;
    }

    internal static void Enable()
    {
        IShowSeedPlugin.Beep.LogWarning($"\n===========================\n\n\n eeeeeNNANABBLLELEE \n\n\n===========================\n");
        Monitor.Enter(_lock);
        _enabled = true;
        _stateBySite.Clear();
        Monitor.Exit(_lock);
    }

    internal static void Disable()
    {
        IShowSeedPlugin.Beep.LogWarning($"\n===========================\n\n\n dissable \n\n\n===========================\n");
        Monitor.Enter(_lock);
        _enabled = false;
        _stateBySite.Clear();
        Monitor.Exit(_lock);
    }

    internal static void Reset()
    {
        IShowSeedPlugin.Beep.LogWarning($"\n===========================\n\n\n reset  RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRReset \n\n\n===========================\n");
        Monitor.Enter(_lock);
        _stateBySite.Clear();
        Monitor.Exit(_lock);
    }

    private static string GetStackTraceStr(int frames)
    {
        var st = new StackTrace(frames);
        return string.Join(" ==> ", st.GetFrames().Select(f =>
        {
            var m = f.GetMethod();
            var dt = m.DeclaringType;
            if (dt == null) return "";
            return $"{dt.FullName}.{m.Name}";
        }));

    }
}
