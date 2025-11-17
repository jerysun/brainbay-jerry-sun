namespace Characters.Commons.MemoryCache;

public interface IMemoryCacheHelper
{
    TResult? GetOrCreate<TResult>(string cacheKey, Func<ICacheEntry, TResult?> valueFactory, int expireSeconds = 300);

    Task<TResult?> GetOrCreateAsync<TResult>(string cacheKey, Func<ICacheEntry, Task<TResult?>> valueFactory, int expireSeconds = 300);

    void Remove(string cacheKey);
}
