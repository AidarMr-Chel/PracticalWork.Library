using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Кэширование с использованием Redis
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// По ключу получает значение из кэша
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task<T> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// По ключу сохраняет значение в кэш с указанным временем жизни
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="ttl"></param>
    /// <returns></returns>
    public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });
    }

    /// <summary>
    /// По ключу удаляет значение из кэша
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public Task RemoveAsync(string key) =>
        _cache.RemoveAsync(key);

    /// <summary>
    /// Регистрирует ключ под реестровым ключом
    /// </summary>
    /// <param name="registryKey"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public async Task TrackKeyAsync(string registryKey, string key)
    {
        var existing = await _cache.GetStringAsync(registryKey);
        var keys = existing != null
            ? JsonSerializer.Deserialize<HashSet<string>>(existing)
            : new HashSet<string>();

        keys.Add(key);

        await _cache.SetStringAsync(registryKey, JsonSerializer.Serialize(keys));
    }

    /// <summary>
    /// Очищает кэш по реестровому ключу
    /// </summary>
    /// <param name="registryKey"></param>
    /// <returns></returns>
    public async Task ClearByRegistryAsync(string registryKey)
    {
        var existing = await _cache.GetStringAsync(registryKey);
        if (existing == null) return;

        var keys = JsonSerializer.Deserialize<HashSet<string>>(existing);
        foreach (var key in keys)
            await _cache.RemoveAsync(key);

        await _cache.RemoveAsync(registryKey);
    }
}
