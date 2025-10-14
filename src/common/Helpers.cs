namespace IShowSeed;

public static class Helpers
{
    public static string LevelOf(UnityEngine.Transform tr)
    {
        if (tr == null) return $"could_not_get_level_of_{tr.gameObject.name}";
        return tr.GetComponentInParent<M_Level>(true).levelName;
    }
}
