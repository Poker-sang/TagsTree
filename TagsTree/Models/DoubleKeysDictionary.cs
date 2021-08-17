using System.Collections.Generic;

namespace TagsTree.Models
{
	public class DoubleKeysDictionary<TKey1, TKey2, TValue> where TKey1 : notnull where TKey2 : notnull where TValue : notnull
	{
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
				if (_dict1.ContainsKey(key1)) return;
				if (_dict2.ContainsKey(key2)) return;
				_dict1[key1] = key2;
				_dict2[key2] = value;
			}
		}
		public TValue this[TKey1 key1] => _dict2[_dict1[key1]];
		public TValue this[TKey2 key2] => _dict2[key2];
		public bool ContainsKey(TKey1 key) => _dict1.ContainsKey(key);
		public bool ContainsKey(TKey2 key) => _dict2.ContainsKey(key);
		public bool ContainsValue(TValue value) => _dict2.ContainsValue(value);
		public bool Remove(TKey1 key1)
		{
			if (!_dict1.ContainsKey(key1)) return false;
			_ = _dict1.Remove(key1);
			_ = _dict2.Remove(_dict1[key1]);
			return true;
		}
		public void Clear()
		{
			_dict1.Clear();
			_dict2.Clear();
		}
	}
}
