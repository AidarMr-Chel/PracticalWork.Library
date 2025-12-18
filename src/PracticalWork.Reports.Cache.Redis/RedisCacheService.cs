using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace PracticalWork.Reports.Cache.Redis;

public class RedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        var json = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(
            key,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            }
        );
    }

    public Task RemoveAsync(string key) =>
        _cache.RemoveAsync(key);

    public async Task TrackKeyAsync(string registryKey, string key)
    {
        var existing = await _cache.GetStringAsync(registryKey);

        var keys = existing != null
            ? JsonSerializer.Deserialize<HashSet<string>>(existing)
            : new HashSet<string>();

        keys!.Add(key);

        await _cache.SetStringAsync(
            registryKey,
            JsonSerializer.Serialize(keys)
        );
    }

    public async Task ClearByRegistryAsync(string registryKey)
    {
        var existing = await _cache.GetStringAsync(registryKey);
        if (existing == null)
            return;

        var keys = JsonSerializer.Deserialize<HashSet<string>>(existing);

        foreach (var key in keys!)
            await _cache.RemoveAsync(key);

        await _cache.RemoveAsync(registryKey);
    }
}
