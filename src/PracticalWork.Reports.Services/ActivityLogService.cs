using PracticalWork.Reports.Entities.Abstractions;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Services.Abstractions;

namespace PracticalWork.Reports.Services;

/// <summary>
/// Сервис чтения логов активности.
/// </summary>
public sealed class ActivityLogService : IActivityLogService
{
    private readonly IActivityLogRepository _repository;

    public ActivityLogService(IActivityLogRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public Task<PagedResult<ActivityLogDto>> GetPagedAsync(
        ActivityLogFilterDto filter,
        CancellationToken cancellationToken = default)
        => _repository.GetPagedAsync(filter, cancellationToken);
}
