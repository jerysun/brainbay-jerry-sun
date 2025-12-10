namespace Characters.Commons.MemoryCache;

public interface IMemoryCacheHelper
{
    public List<string> CachedKeys { get; set; }

    TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory, int expireSeconds = 300);

    Task<(TResult? result, bool)>  GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory, int expireSeconds = 300);

    void Remove(string cacheKey);

    void ClearAll();
}
