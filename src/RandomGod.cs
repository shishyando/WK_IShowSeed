using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

namespace IShowSeed.Random;

public static class Rod // short for RandomGod
{
    private static readonly object _lock = new();
    private static readonly Dictionary<int, UnityEngine.Random.State> _stateBySiteSeed = [];
    private static readonly Dictionary<int, uint> _cntBySiteSeed = [];
    private static bool _enabled;

    public struct Context
    {
        public UnityEngine.Random.State PrevRandomState;
        public int BaseSeed;
        public bool Valid;
        public uint CallNumber;
    }

    internal static void Enter(ref Context ctx)
    {
        if (!_enabled) return;
        ctx = default; 
        ctx.BaseSeed = DeriveSeed(ComputeSiteKey());
        Monitor.Enter(_lock);
        ctx.PrevRandomState = UnityEngine.Random.state;

        if (!_stateBySiteSeed.ContainsKey(ctx.BaseSeed))
        {
            // create a new Random.State which will advance in future
            // have to InitState with a derived seed to save it
            UnityEngine.Random.InitState(ctx.BaseSeed);
            _stateBySiteSeed[ctx.BaseSeed] = UnityEngine.Random.state;
            _cntBySiteSeed[ctx.BaseSeed] = 1;
            ctx.CallNumber = 1;
        }
        else
        {
            UnityEngine.Random.state = _stateBySiteSeed[ctx.BaseSeed];
            ctx.CallNumber = ++_cntBySiteSeed[ctx.BaseSeed];
        }

        ctx.Valid = true;
    }

    internal static void Exit(in Context ctx)
    {
        if (!ctx.Valid) return;
        var newRandomStateAfterCallToSaveToMap = UnityEngine.Random.state;
        string newRandomStateAfterCallToSaveToMapStr = JsonUtility.ToJson(newRandomStateAfterCallToSaveToMap);
        string mapStateBeforeCallStr = JsonUtility.ToJson(_stateBySiteSeed[ctx.BaseSeed]);
        _stateBySiteSeed[ctx.BaseSeed] = newRandomStateAfterCallToSaveToMap;
        UnityEngine.Random.state = ctx.PrevRandomState;
        Monitor.Exit(_lock);
    }

    private static string ComputeSiteKey()
    {
        string traceStr = GetStackTraceStr(2);
        return traceStr;
    }

    private static int DeriveSeed(string siteKey)
    {
        return Animator.StringToHash(siteKey) ^ IShowSeedPlugin.StartingSeed;
    }

    internal static void Enable()
    {
        IShowSeedPlugin.Beep.LogInfo($"RandomGod enabled");
        Monitor.Enter(_lock);
        _enabled = true;
        _stateBySiteSeed.Clear();
        Monitor.Exit(_lock);
    }

    internal static void Disable()
    {
        IShowSeedPlugin.Beep.LogWarning($"RandomGod disabled");
        Monitor.Enter(_lock);
        _enabled = false;
        _stateBySiteSeed.Clear();
        Monitor.Exit(_lock);
    }

    internal static void Reset()
    {
        IShowSeedPlugin.Beep.LogWarning($"RandomGod reset");
        Monitor.Enter(_lock);
        _stateBySiteSeed.Clear();
        Monitor.Exit(_lock);
    }

    internal static bool IsEnabled()
    {
        return _enabled;
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
