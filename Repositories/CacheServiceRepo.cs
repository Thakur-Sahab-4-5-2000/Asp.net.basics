using System.Collections.Concurrent;

public class CacheService
{
    private readonly ConcurrentDictionary<string, CacheItem> _cache = new ConcurrentDictionary<string, CacheItem>();

    public CacheService() { }

    public void AddOrUpdateCache(string key, object value, TimeSpan expirationTime)
    {
        var cacheItem = new CacheItem
        {
            Value = value,
            ExpirationTime = DateTime.UtcNow.Add(expirationTime)
        };

        _cache[key] = cacheItem;
    }

    public T GetCache<T>(string key)
    {
        if (_cache.TryGetValue(key, out var cacheItem))
        {
            if (cacheItem.ExpirationTime > DateTime.UtcNow)
            {
                return (T)cacheItem.Value;
            }
            else
            {
                _cache.TryRemove(key, out _);
            }
        }

        return default;
    }

    public void RemoveCache(string key)
    {
        _cache.TryRemove(key, out _);
    }

    private class CacheItem
    {
        public object Value { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
