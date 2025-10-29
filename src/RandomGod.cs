using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace IShowSeed.Random;

public static class Rod // short for RandomGod
{
    public enum ERandomMode
    {
        Disabled,
        Enabled,
        Prediction,
    }

    public struct Context
    {
        public UnityEngine.Random.State PrevRandomState;
        public int BaseSeed;
        public bool Valid;
        public uint CallNumber;
    }

    public static ERandomMode Mode = ERandomMode.Disabled;

    private static readonly object _lock = new();
    private static readonly Dictionary<int, UnityEngine.Random.State> _stateBySiteSeed = [];
    private static readonly Dictionary<int, uint> _cntBySiteSeed = [];


    internal static void Enter(ref Context ctx, string customCallSite)
    {
        if (Mode == ERandomMode.Disabled) return;
        ctx = default;
        ctx.BaseSeed = DeriveSeed(customCallSite ?? ComputeSiteKey());
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
        return Animator.StringToHash(siteKey) ^ Plugin.SeedForRandom;
    }

    internal static ERandomMode GetMode()
    {
        return Mode;
    }

    internal static void SwitchToMode(ERandomMode mode)
    {
        if (Mode != ERandomMode.Prediction && mode != ERandomMode.Prediction) // less log spam
        {
            Plugin.Beep.LogInfo($"RandomGod switching to {mode}");
        }
        Monitor.Enter(_lock);
        Mode = mode;
        _stateBySiteSeed.Clear();
        Monitor.Exit(_lock);
    }

    internal static void Reset()
    {
        Plugin.Beep.LogWarning($"RandomGod reset");
        Monitor.Enter(_lock);
        _stateBySiteSeed.Clear();
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
