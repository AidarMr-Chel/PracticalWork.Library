using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Entities;
using PracticalWork.Reports.Entities.Abstractions;
using PracticalWork.Reports.Entities.DTO;

namespace PracticalWork.Reports.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с логами активности.
/// </summary>
public sealed class ActivityLogRepository : IActivityLogRepository
{
    private readonly ReportsDbContext _db;

    public ActivityLogRepository(ReportsDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task<List<ActivityLog>> GetLogsAsync(
        DateOnly from,
        DateOnly to,
        string? eventType,
        CancellationToken cancellationToken = default)
    {
        var fromDate = from.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        var toDate = to.ToDateTime(TimeOnly.MaxValue).ToUniversalTime();

        var query = _db.ActivityLogs
            .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(x => x.EventType == eventType);

        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<PagedResult<ActivityLogDto>> GetPagedAsync(
        ActivityLogFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var query = _db.ActivityLogs.AsQueryable();

        if (filter.From.HasValue)
            query = query.Where(x => x.CreatedAt >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(x => x.CreatedAt <= filter.To.Value);

        if (!string.IsNullOrWhiteSpace(filter.EventType))
            query = query.Where(x => x.EventType == filter.EventType);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(x => new ActivityLogDto
            {
                Id = x.Id,
                EventType = x.EventType,
                Payload = x.Payload,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ActivityLogDto>
        {
            Items = items,
            Total = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    /// <inheritdoc />
    public async Task AddAsync(ActivityLog log, CancellationToken cancellationToken = default)
    {
        _db.ActivityLogs.Add(log);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
