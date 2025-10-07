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
        // IShowSeedPlugin.Beep.LogInfo($"ROD: Enter called");
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
        // IShowSeedPlugin.Beep.LogInfo($"ROD: Enter COMPLETED!, {ctx.SiteKey}");
    }

    internal static void Exit(in Context ctx)
    {
        // IShowSeedPlugin.Beep.LogInfo($"ROD: Exit called, ctx.Valid={ctx.Valid}");
        if (!_enabled || !ctx.Valid) return;
        var newState = UnityEngine.Random.state;
        string newStateAfterCallToSave = JsonUtility.ToJson(newState);
        string pulledBeforeCallStateStr = JsonUtility.ToJson(_stateBySiteSeed[ctx.BaseSeed]);
        if (newStateAfterCallToSave == pulledBeforeCallStateStr)
        {
            IShowSeedPlugin.Beep.LogInfo($"\n################################\n unchanged states:\npulledBeforeCall={pulledBeforeCallStateStr}\nnewStateAfterCallToSave={newStateAfterCallToSave}\nsite={GetStackTraceStr(2)}\n\n################################\n");
        }
        _stateBySiteSeed[ctx.BaseSeed] = newState;
        if (GetStackTraceStr(2).Contains("App_PerkPage"))
        {
            IShowSeedPlugin.Beep.LogWarning($"=========================== ROD exit completed with seed {ctx.BaseSeed} (callNumber={ctx.CallNumber})");
            IShowSeedPlugin.Beep.LogWarning($"\npulledBeforeCall: {pulledBeforeCallStateStr}\nnewStateAfterCallToSave: {newStateAfterCallToSave}");
            IShowSeedPlugin.Beep.LogWarning($"new random state map:\n\t{_stateBySiteSeed.Select(kvp => { return $"s={kvp.Key}, r={JsonUtility.ToJson(kvp.Value)}"; }).Join(delimiter: "\n\t")}");
            IShowSeedPlugin.Beep.LogWarning($"new call number map:\n{JsonConvert.SerializeObject(_cntBySiteSeed)}");
            IShowSeedPlugin.Beep.LogWarning($"\nsitekey:{GetStackTraceStr(2)}");
        }
        UnityEngine.Random.state = ctx.PrevRandomState;
        Monitor.Exit(_lock);
    }

    private static string ComputeSiteKey()
    {
        string traceStr = GetStackTraceStr(2);
        // IShowSeedPlugin.Beep.LogInfo($"ROD: ComputeSiteKey is called");

        return traceStr;
    }

    private static int DeriveSeed(string siteKey)
    {
        return Animator.StringToHash(siteKey) ^ IShowSeedPlugin.StartingSeed;
    }

    internal static void Enable()
    {
        IShowSeedPlugin.Beep.LogWarning($"\n===========================\n\n\n enable eeeeeNNANABBLLELEE qwe \n\n\n===========================\n");
        Monitor.Enter(_lock);
        _enabled = true;
        _stateBySiteSeed.Clear();
        Monitor.Exit(_lock);
    }

    internal static void Disable()
    {
        IShowSeedPlugin.Beep.LogWarning($"\n===========================\n\n\n disable asdasd \n\n\n===========================\n");
        Monitor.Enter(_lock);
        _enabled = false;
        _stateBySiteSeed.Clear();
        Monitor.Exit(_lock);
    }

    internal static void Reset()
    {
        IShowSeedPlugin.Beep.LogWarning($"\n===========================\n\n\n reset  RRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRReset zxc \n\n\n===========================\n");
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
