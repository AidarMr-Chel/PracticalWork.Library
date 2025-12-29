using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Entities;

namespace PracticalWork.Reports.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с логами активности.
/// Позволяет получать записи за период и фильтровать их по типу события.
/// </summary>
public class ActivityLogRepository
{
    private readonly ReportsDbContext _db;

    public ActivityLogRepository(ReportsDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Возвращает список логов активности за указанный период.
    /// При необходимости выполняет фильтрацию по типу события.
    /// </summary>
    /// <param name="from">Начальная дата периода (включительно).</param>
    /// <param name="to">Конечная дата периода (включительно).</param>
    /// <param name="eventType">Тип события для фильтрации или null, чтобы вернуть все события.</param>
    /// <returns>Список логов активности, удовлетворяющих условиям фильтрации.</returns>
    public async Task<List<ActivityLog>> GetLogsAsync(DateOnly from, DateOnly to, string? eventType)
    {
        var fromDate = from.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        var toDate = to.ToDateTime(TimeOnly.MaxValue).ToUniversalTime();

        var query = _db.ActivityLogs
            .Where(x => x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(x => x.EventType == eventType);

        return await query.ToListAsync();
    }
}
