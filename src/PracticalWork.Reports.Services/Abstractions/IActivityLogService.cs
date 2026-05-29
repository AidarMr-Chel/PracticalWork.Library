using PracticalWork.Reports.Entities.DTO;

namespace PracticalWork.Reports.Services.Abstractions;

/// <summary>
/// Сервис чтения логов активности.
/// </summary>
public interface IActivityLogService
{
    Task<PagedResult<ActivityLogDto>> GetPagedAsync(ActivityLogFilterDto filter, CancellationToken cancellationToken = default);
}
