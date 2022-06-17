using System;
using System.Collections.Generic;
using System.Linq;

namespace TagsTree.Algorithm;

public class DoubleKeysDictionary<TKey1, TKey2, TValue> where TKey1 : notnull where TKey2 : notnull where TValue : notnull
{
    public DoubleKeysDictionary()
    {
        if (typeof(TKey1) == typeof(TKey2))
            throw new ArgumentException($"DoubleKeysDictionary的两个键类型不能相同，此处类型都为{typeof(TKey1).Name}");
    }
    private readonly Dictionary<TKey1, TKey2> _dict1 = new();
    private readonly Dictionary<TKey2, TValue> _dict2 = new();
    public int Count => _dict1.Count;
    public Dictionary<TKey1, TKey2>.KeyCollection Keys1 => _dict1.Keys;
    public Dictionary<TKey2, TValue>.KeyCollection Keys2 => _dict2.Keys;
    public Dictionary<TKey2, TValue>.ValueCollection Values => _dict2.Values;
    public TValue this[TKey1 key1, TKey2 key2]
    {
        set
        {
            if (_dict1.ContainsKey(key1) || _dict2.ContainsKey(key2))
                return;
            _dict1[key1] = key2;
            _dict2[key2] = value;
        }
    }
    public void ChangeKey2(TKey2 oldKey2, TKey2 newKey2)
    {
        if (!_dict2.ContainsKey(oldKey2))
            return;
        foreach (var pair in _dict1.Where(pair => Equals(pair.Value, oldKey2)))
            _dict1[pair.Key] = newKey2;
        _ = _dict2.Remove(oldKey2, out var value);
        _dict2[newKey2] = value!;
    }
    public TValue this[TKey1 key1] => _dict2[_dict1[key1]];
    public TValue this[TKey2 key2] => _dict2[key2];
    public TValue? GetValueOrDefault(TKey1 key1) => _dict1.ContainsKey(key1) ? _dict2[_dict1[key1]] : default;
    public TValue? GetValueOrDefault(TKey2 key2) => _dict2.ContainsKey(key2) ? _dict2[key2] : default;

    public bool ContainsKey(TKey1 key1) => _dict1.ContainsKey(key1);
    public bool ContainsKey(TKey2 key2) => _dict2.ContainsKey(key2);
    public bool ContainsValue(TValue value) => _dict2.ContainsValue(value);
    public bool Remove(TKey1 key1)
    {
        if (!_dict1.ContainsKey(key1))
            return false;
        _ = _dict2.Remove(_dict1[key1]);
        _ = _dict1.Remove(key1);
        return true;
    }
    public void Clear()
    {
        _dict1.Clear();
        _dict2.Clear();
    }
}