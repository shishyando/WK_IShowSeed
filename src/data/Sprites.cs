using System;
using System.Collections.Generic;
using UnityEngine;

namespace IShowSeed.HandleProviders;

public class Sprites : HandleProvider<Sprite>
{
    public override Func<Sprite, string> Name {get;} = x => x?.name?.ToLower() ?? null;
    private List<Sprite> _sprites = [];

    public void InitializeFromAssetDatabase()
    {
        _sprites = CL_AssetManager.GetFullCombinedAssetDatabase().spriteAssets;
    }

    public override Handle<Sprite> Handle()
    {
        if (_sprites.Count == 0) InitializeFromAssetDatabase();
        return new(_sprites, Name, Finalizer);
    }
}
