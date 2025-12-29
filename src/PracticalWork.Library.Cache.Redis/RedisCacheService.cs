using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Кэширование с использованием Redis.
/// Сериализация и десериализация выполняются через System.Text.Json.
/// </summary>
public class RedisCacheService : ICacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// По ключу получает значение из кэша.
    /// Десериализация выполняется через System.Text.Json.
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
    /// <param name="key">Ключ кэша.</param>
    /// <returns>
    /// Значение из кэша или значение по умолчанию, если ключ не найден.
    /// </returns>
    public async Task<T> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// По ключу сохраняет значение в кэш с указанным временем жизни.
    /// Сериализация выполняется через System.Text.Json.
    /// </summary>
    /// <typeparam name="T">Тип сохраняемого значения.</typeparam>
    /// <param name="key">Ключ кэша.</param>
    /// <param name="value">Сохраняемое значение.</param>
    /// <param name="ttl">Время жизни записи.</param>
    /// <returns>Задача сохранения значения в кэш.</returns>
    public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        var json = JsonSerializer.Serialize(value);
        await _cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = ttl
        });
    }

    /// <summary>
    /// По ключу удаляет значение из кэша.
    /// </summary>
    /// <param name="key">Ключ кэша.</param>
    /// <returns>Задача удаления значения из кэша.</returns>
    public Task RemoveAsync(string key) =>
        _cache.RemoveAsync(key);

    /// <summary>
    /// Регистрирует ключ под реестровым ключом.
    /// Список ключей сериализуется через System.Text.Json.
    /// </summary>
    /// <param name="registryKey">Ключ реестра.</param>
    /// <param name="key">Добавляемый ключ.</param>
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
    /// Очищает кэш по реестровому ключу.
    /// Список ключей десериализуется через System.Text.Json.
    /// </summary>
    /// <param name="registryKey">Ключ реестра.</param>
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
