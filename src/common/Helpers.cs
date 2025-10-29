using System.Collections.Generic;
using HarmonyLib;

namespace IShowSeed;

public static class Helpers
{
    public static readonly AccessTools.FieldRef<CL_AssetManager, List<CL_AssetManager.WKDatabaseHolder>> databasesRef
        = AccessTools.FieldRefAccess<List<CL_AssetManager.WKDatabaseHolder>>(typeof(CL_AssetManager), "databases");

    public static string LevelOf(UnityEngine.Transform tr)
    {
        if (tr == null) return $"could_not_get_level_of_{tr.gameObject.name}";
        return tr.GetComponentInParent<M_Level>(true).levelName;
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
}
