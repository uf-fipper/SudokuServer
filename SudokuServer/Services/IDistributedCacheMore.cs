using Microsoft.Extensions.Caching.Distributed;

namespace SudokuServer.Services;

public interface IDistributedCacheMore : IDistributedCache
{
    async Task<T?> GetJsonAsync<T>(string key, CancellationToken token = default)
    {
        var data = await GetAsync(key, token);
        if (data == null)
            return default;
        return System.Text.Json.JsonSerializer.Deserialize<T>(data);
    }

    async Task SetJsonAsync<T>(
        string key,
        T value,
        DistributedCacheEntryOptions options,
        CancellationToken token = default
    )
    {
        var data = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(value);
        await SetAsync(key, data, options, token);
    }
}
