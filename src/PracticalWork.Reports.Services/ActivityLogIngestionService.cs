using PracticalWork.Reports.Entities;
using PracticalWork.Reports.Entities.Abstractions;
using PracticalWork.Reports.Services.Abstractions;

namespace PracticalWork.Reports.Services;

/// <summary>
/// Сохраняет события активности, полученные из брокера сообщений.
/// </summary>
public sealed class ActivityLogIngestionService : IActivityLogIngestionService
{
    private readonly IActivityLogRepository _repository;

    public ActivityLogIngestionService(IActivityLogRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public Task IngestAsync(string eventType, string payload, CancellationToken cancellationToken = default)
    {
        var log = new ActivityLog
        {
            EventType = eventType,
            Payload = payload,
            CreatedAt = DateTime.UtcNow
        };

        return _repository.AddAsync(log, cancellationToken);
    }
}
