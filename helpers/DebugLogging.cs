


using System;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace IShowSeed.Helpers;

public static class DebugHelper {

    public static void LogTransform(Transform tr, string className)
    {
        GameObject obj = tr.gameObject;
        string selfName = obj.name;

        var parent = tr.parent;
        string parentName = parent != null ? parent.gameObject.name : "<root>";
        string fullPath = GetPath(tr);
        int objId = obj.GetInstanceID();
        int siblingIdx = obj.transform.GetSiblingIndex();

        var st = new StackTrace(skipFrames: 1, fNeedFileInfo: true);
        string trace = string.Join("\n", st.GetFrames()?
            .Select(f => f.ToString())
            .Where(s => s != null &&
                        !s.Contains("HarmonyLib") &&
                        !s.Contains(nameof(LogTransform)))
            ?? Array.Empty<string>());

        IShowSeedPlugin.Logger.LogWarning(
            $"{className}:" +
            $"\n\tname\t=\t{selfName}" +
            $"\n\tparent\t=\t{parentName}" +
            $"\n\tpath\t=\t{fullPath}" +
            $"\n\tactive\t=\t{obj.activeSelf}" +
            $"\n\tid\t=\t{objId}" +
            $"\n\tsiblingIdx\t=\t{siblingIdx}" +
            $"\n\tStackTrace:" +
            $"\n{trace}" +
            "=========="
        );
    }


    private static string GetPath(Transform t)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder(t.name);
        for (Transform p = t.parent; p != null; p = p.parent)
            sb.Insert(0, p.name + "/");
        return sb.ToString();
    }

};
