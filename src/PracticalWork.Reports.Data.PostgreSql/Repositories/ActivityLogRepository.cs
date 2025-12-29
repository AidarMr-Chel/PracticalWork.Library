using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Entities;

namespace PracticalWork.Reports.Data.PostgreSql.Repositories;

public class ActivityLogRepository
{
    private readonly ReportsDbContext _db;

    public ActivityLogRepository(ReportsDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Возвращает логи активности за указанный период с возможностью фильтрации по типу события
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="eventType"></param>
    /// <returns></returns>
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
