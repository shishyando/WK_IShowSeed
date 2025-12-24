using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace IShowSeed;

public static class Helpers
{
    public static readonly AccessTools.FieldRef<CL_AssetManager, List<CL_AssetManager.WKDatabaseHolder>> databasesRef
        = AccessTools.FieldRefAccess<List<CL_AssetManager.WKDatabaseHolder>>(typeof(CL_AssetManager), "databases");

    public static string LevelOfStr(UnityEngine.Transform tr)
    {
        if (tr == null) return $"could_not_get_level_of_{tr.gameObject.name}";
        return tr.GetComponentInParent<M_Level>(true).levelName;
    }
    public static M_Level LevelOf(UnityEngine.Transform tr)
    {
        if (tr == null) return null;
        return tr.GetComponentInParent<M_Level>(true);
    }

    public static List<Perk> GetAllPerks() // GetFullCombinedAssetDatabase spams in logs
    {
        List<Perk> result = [];
        foreach (CL_AssetManager.WKDatabaseHolder wkdatabaseHolder in databasesRef(CL_AssetManager.instance))
        {
            result.AddRange(wkdatabaseHolder.database.perkAssets);
        }
        foreach (Perk p in result)
        {
            p.id = p.id.ToLower();
        }
        return result;
    }


    internal static void PatchAllWithAttribute<T>(Harmony harmony) where T : Attribute
    {
        var assembly = Assembly.GetExecutingAssembly();
        var patches = assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<T>() != null);
        foreach (var p in patches)
        {
            harmony.PatchAll(p);
        }
    }

    internal static List<string> BaseGamemodes = [
        "Campaign",
        "Endless Superstructure",
        "Endless Substructure",
        "Endless Underworks",
        "Endless Silos",
        "Endless Pipeworks",
        "Endless Habitation",
        "Endless Abyss",
    ];

    internal static List<string> GetAllGamemodes(bool withOptions)
    {
        List<string> res = [];
        foreach (var g in BaseGamemodes)
        {
            res.Add(g);
            if (withOptions)
            {
                res.Add(g + "-Hardmode");
                res.Add(g + "-Iron");
                res.Add(g + "-Hardmode-Iron");
            }
        }
        return res;
    }

    internal static string GetAllGamemodesStr(bool withOptions, string delimiter)
    {
        return String.Join(delimiter, GetAllGamemodes(withOptions));
    }

}


[AttributeUsage(AttributeTargets.Class)]
public class OnlyForSeededRunsPatchAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public class PermanentPatchAttribute : Attribute { }


