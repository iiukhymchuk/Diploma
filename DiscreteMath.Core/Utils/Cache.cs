using System.Collections.Generic;

namespace DiscreteMath.Core.Utils
{
    public class Cache<TKey, TSecondaryKey, TValue>
    {
        private readonly Dictionary<TKey, Dictionary<TSecondaryKey, TValue>> _cache = new Dictionary<TKey, Dictionary<TSecondaryKey, TValue>>();

        public void Store(TKey key, TSecondaryKey secondaryKey, TValue value)
        {
            if (!_cache.TryGetValue(key, out _))
                _cache[key] = new Dictionary<TSecondaryKey, TValue>();

            _cache[key][secondaryKey] = value;
        }

        public bool TryGetValue(TKey key, TSecondaryKey secondaryKey, out TValue value)
        {
            if (!_cache.ContainsKey(key) || !_cache[key].ContainsKey(secondaryKey))
            {
                value = default;
                return false;
            }

            value = _cache[key][secondaryKey];
            return true;
        }
    }
}