using System;
using System.Collections.Generic;
using System.Linq;
using IShowSeed.HandleProviders;
using UnityEngine;

namespace IShowSeed;

public static class Things
{
    private static readonly Items _items = new();
    private static readonly Sprites _sprites = new();
    
    public static Handle<Item_Object> Items()
    {
        return _items.Handle();
    }

    public static Handle<Sprite> Sprites()
    {
        return _sprites.Handle();
    }
}

public abstract class HandleProvider<T>
{
    public abstract Func<T, string> Name {get;}
    public virtual Func<T, T> Finalizer {get;} = obj => obj;
    public abstract Handle<T> Handle();
}

public class Handle<T>(IEnumerable<T> d, Func<T, string> nameGetter, Func<T, T> finalizer)
{
    private readonly IEnumerable<T> _data = d;
    private readonly Func<T, string> _nameGetter = x => nameGetter(x)?.ToLower() ?? null;
    private readonly Func<T, T> _finalizer = finalizer;

    public Handle<T> Filter(string filter)
    {
        return new(_data.Where(x => _nameGetter(x).Contains(filter.ToLower())), _nameGetter, _finalizer);
    }

    public T Any()
    {
        var result = _data.FirstOrDefault();
        return result != null ? _finalizer(result) : default;
    }

    public string AnyName()
    {
        return _nameGetter(Any());
    }

    public string[] Names()
    {
        return [.. _data.Select(x => _nameGetter(x))];
    }

    public string Join(string delimiter = "\n- ")
    {
        return string.Join(delimiter, Names());
    }

    public List<T> ToList()
    {
        return [.. _data];
    }

    public IEnumerable<T> Data()
    {
        return _data;
    }

    public int Count()
    {
        return _data.Count();
    }
}
