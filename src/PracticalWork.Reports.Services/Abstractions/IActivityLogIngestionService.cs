namespace PracticalWork.Reports.Services.Abstractions;

/// <summary>
/// Сервис сохранения событий из брокера сообщений.
/// </summary>
public interface IActivityLogIngestionService
{
    Task IngestAsync(string eventType, string payload, CancellationToken cancellationToken = default);
}
