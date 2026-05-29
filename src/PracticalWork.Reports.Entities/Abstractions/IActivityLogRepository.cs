using PracticalWork.Reports.Entities.DTO;

namespace PracticalWork.Reports.Entities.Abstractions;

/// <summary>
/// Порт доступа к логам активности.
/// </summary>
public interface IActivityLogRepository
{
    Task<List<ActivityLog>> GetLogsAsync(DateOnly from, DateOnly to, string? eventType, CancellationToken cancellationToken = default);

    Task<PagedResult<ActivityLogDto>> GetPagedAsync(ActivityLogFilterDto filter, CancellationToken cancellationToken = default);

    Task AddAsync(ActivityLog log, CancellationToken cancellationToken = default);
}
