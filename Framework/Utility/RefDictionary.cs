using System.Collections.Generic;

namespace Arenbee.Framework.Utility
{
    public class RefDictionary<TKey>
    {
        public RefDictionary()
        {
            _dictionary = new();
        }

        private readonly Dictionary<TKey, int> _dictionary;

        public int this[TKey key]
        {
            get => _dictionary[key];
            set { _dictionary[key] = value; }
        }

        public void Add(TKey key)
        {
            if (_dictionary.ContainsKey(key))
                _dictionary[key]++;
            else
                _dictionary[key] = 1;
        }

        public void Remove(TKey key)
        {
            if (_dictionary.TryGetValue(key, out int value))
            {
                if (value - 1 <= 0)
                    _dictionary.Remove(key);
                else
                    _dictionary[key]--;
            }
        }

        public void RemoveAll(TKey key)
        {
            _dictionary.Remove(key);
        }

        public void Clear()
        {
            _dictionary.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out int value)
        {
            return _dictionary.TryGetValue(key, out value);
        }
    }
}