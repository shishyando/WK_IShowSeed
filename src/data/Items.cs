using System;
using System.Collections.Generic;

namespace IShowSeed.HandleProviders;

public class Items : HandleProvider<Item_Object>
{
    public override Func<Item_Object, string> Name {get;} = x => x?.name?.ToLower() ?? null;
    private List<Item_Object> _items = [];

    public void InitializeFromAssetDatabase()
    {
        var itemPrefabs = CL_AssetManager.GetFullCombinedAssetDatabase().itemPrefabs;
        foreach (UnityEngine.GameObject prefab in itemPrefabs)
        {
            Item_Object component = prefab.GetComponent<Item_Object>();
            if (component != null)
            {
                _items.Add(component);
            }
        }
    }

    public override Handle<Item_Object> Handle()
    {
        if (_items.Count == 0) InitializeFromAssetDatabase();
        return new(_items, Name, Finalizer);
    }
}
