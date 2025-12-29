using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace PracticalWork.Reports.Cache.Redis;

/// <summary>
/// Сервис кэширования для отчётов, основанный на Redis.
/// Использует <see cref="IDistributedCache"/> и сериализацию через <see cref="JsonSerializer"/>.
/// </summary>
public class RedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Получает значение из кэша по указанному ключу.
    /// Десериализация выполняется через <see cref="JsonSerializer"/>.
    /// </summary>
    /// <typeparam name="T">Тип возвращаемого значения.</typeparam>
    /// <param name="key">Ключ кэша.</param>
    /// <returns>
    /// Значение типа <typeparamref name="T"/> или <c>null</c>, если ключ отсутствует.
    /// </returns>
    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _cache.GetStringAsync(key);
        return json == null ? default : JsonSerializer.Deserialize<T>(json);
    }

    /// <summary>
    /// Сохраняет значение в кэш под указанным ключом с заданным временем жизни.
    /// Сериализация выполняется через <see cref="JsonSerializer"/>.
    /// </summary>
    /// <typeparam name="T">Тип сохраняемого значения.</typeparam>
    /// <param name="key">Ключ кэша.</param>
    /// <param name="value">Сохраняемое значение.</param>
    /// <param name="ttl">Время жизни записи.</param>
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

    /// <summary>
    /// Удаляет значение из кэша по указанному ключу.
    /// </summary>
    /// <param name="key">Ключ кэша.</param>
    public Task RemoveAsync(string key) =>
        _cache.RemoveAsync(key);

    /// <summary>
    /// Добавляет ключ в реестр, позволяющий группировать связанные записи.
    /// Реестр хранится как JSON‑множество строк.
    /// </summary>
    /// <param name="registryKey">Ключ реестра.</param>
    /// <param name="key">Добавляемый ключ.</param>
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

    /// <summary>
    /// Удаляет все ключи, зарегистрированные под указанным реестровым ключом,
    /// а затем удаляет сам реестр.
    /// </summary>
    /// <param name="registryKey">Ключ реестра.</param>
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
