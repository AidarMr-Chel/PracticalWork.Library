namespace PracticalWork.Reports.Entities.Abstractions;

/// <summary>
/// Порт кэширования для модуля отчётов.
/// </summary>
public interface IReportsCacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken cancellationToken = default);

    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}
