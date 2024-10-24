using System.Collections.Generic;
using TagsTree.Services;

namespace TagsTree.Algorithm;

/// <summary>
/// 包含两个不同类型的双向字典
/// </summary>
public class BidirectionalDictionary<TKey, TValue> where TKey : notnull where TValue : notnull
{
    private readonly Dictionary<TKey, TValue> _dict1 = [];

    private readonly Dictionary<TValue, TKey> _dict2 = [];

    public int Count => _dict1.Count;

    public Dictionary<TKey, TValue>.KeyCollection Keys => _dict1.Keys;

    public Dictionary<TValue, TKey>.KeyCollection Values => _dict2.Keys;

    public TValue this[TKey key]
    {
        get => _dict1[key];
        set
        {
            if (!_dict1.TryAdd(key, value))
                return;
            _dict2[value] = key;
        }
    }

    public TKey this[TValue key]
    {
        get => _dict2[key];
        set
        {
            if (_dict2.ContainsKey(key))
                return;
            _dict1[value] = key;
            _dict2[key] = value;
        }
    }

    public bool Contains(TKey key) => _dict1.ContainsKey(key);

    public bool Contains(TValue key) => _dict2.ContainsKey(key);

    public bool Remove(TKey key)
    {
        if (!_dict1.TryGetValue(key, out var value))
            return false;
        _ = _dict2.Remove(value);
        _ = _dict1.Remove(key);
        return true;
    }

    public bool Remove(TValue key)
    {
        if (!_dict2.TryGetValue(key, out var value))
            return false;
        _ = _dict1.Remove(value);
        _ = _dict2.Remove(key);
        return true;
    }

    public void Deserialize(string path)
    {
        _dict1.Clear();
        _dict2.Clear();
        foreach (var (key, value) in Serialization.Deserialize<Dictionary<TKey, TValue>>(path))
        {
            _dict1[key] = value;
            _dict2[value] = key;
        }
    }

    public void Serialize(string path) => Serialization.Serialize(path, _dict1);
}
