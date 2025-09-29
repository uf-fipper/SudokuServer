using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using SudokuServer.Services;

namespace SudokuServer.ServicesImpl;

public class RedisDistributedCache(IDatabase redis) : IDistributedCacheMore
{
    public byte[]? Get(string key)
    {
        return redis.StringGet(key);
    }

    public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
    {
        return await redis.StringGetAsync(key);
    }

    public void Refresh(string key)
    {
        return;
    }

    public Task RefreshAsync(string key, CancellationToken token = default)
    {
        return Task.CompletedTask;
    }

    public void Remove(string key)
    {
        redis.KeyDelete(key);
    }

    public Task RemoveAsync(string key, CancellationToken token = default)
    {
        return redis.KeyDeleteAsync(key);
    }

    public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
    {
        redis.StringSet(key, value, GetExpiration(options));
    }

    public Task SetAsync(
        string key,
        byte[] value,
        DistributedCacheEntryOptions options,
        CancellationToken token = default
    )
    {
        return redis.StringSetAsync(key, value, GetExpiration(options));
    }

    private TimeSpan? GetExpiration(DistributedCacheEntryOptions options)
    {
        if (options.AbsoluteExpirationRelativeToNow.HasValue)
        {
            return options.AbsoluteExpirationRelativeToNow;
        }
        if (options.AbsoluteExpiration.HasValue)
        {
            return options.AbsoluteExpiration.Value - DateTimeOffset.Now;
        }
        return null;
    }
}
