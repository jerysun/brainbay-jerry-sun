namespace Characters.Commons.MemoryCache;

public class MemoryCacheHelper : IMemoryCacheHelper
{
    private readonly IMemoryCache _memoryCache;

    public List<string> CachedKeys { get; set; } = [];

    public MemoryCacheHelper(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    private static void InitCacheEntry(ICacheEntry entry, int baseExpireSeconds)
    {
        double sec = Random.Shared.Next(baseExpireSeconds, baseExpireSeconds * 2);
        TimeSpan expiration = TimeSpan.FromSeconds(sec);
        entry.SlidingExpiration = TimeSpan.FromSeconds(baseExpireSeconds);
        entry.AbsoluteExpirationRelativeToNow = expiration;
    }

    public TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory, int baseExpireSeconds = 300)
    {
        if (_memoryCache.TryGetValue(cacheKey, out TResult? result)) return result;
        CachedKeys.Add(cacheKey);
        using ICacheEntry entry = _memoryCache.CreateEntry(cacheKey);
        InitCacheEntry(entry, baseExpireSeconds);
        result = valueFactory(entry)!;
        entry.Value = result;
        return result;
    }

    public async Task<(TResult? result, bool)> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory, int baseExpireSeconds = 60)
    {
        if (_memoryCache.TryGetValue(cacheKey, out TResult? result)) return (result, true);
        CachedKeys.Add(cacheKey);
        using ICacheEntry entry = _memoryCache.CreateEntry(cacheKey);
        InitCacheEntry(entry, baseExpireSeconds);
        result = (await valueFactory(entry))!;
        entry.Value = result;
        return (result, false);
    }

    public void Remove(string cacheKey)
    {
        _memoryCache.Remove(cacheKey);
    }

    public void ClearAll()
    {
        foreach (var key in CachedKeys)
        {
            Remove(key);
        }
    }
}
