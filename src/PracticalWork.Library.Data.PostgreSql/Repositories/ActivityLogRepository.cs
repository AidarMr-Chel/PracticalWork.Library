using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с логами активности в отчётной базе данных.
/// Выполняет выборку данных и маппинг EF‑сущностей в DTO ядра.
/// </summary>
public sealed class ActivityLogRepository : IActivityLogRepository
{
    private readonly ReportsDbContext _context;

    /// <summary>
    /// Создаёт экземпляр репозитория логов активности.
    /// </summary>
    /// <param name="context">Контекст отчётной базы данных.</param>
    public ActivityLogRepository(ReportsDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Возвращает логи активности за указанный диапазон дат.
    /// Маппинг выполняется на уровне БД для оптимальной производительности.
    /// </summary>
    /// <param name="start">Начальная дата диапазона (включительно).</param>
    /// <param name="end">Конечная дата диапазона (включительно).</param>
    /// <param name="ct">Токен отмены.</param>
    /// <returns>Коллекция DTO логов активности.</returns>
    public async Task<IEnumerable<ActivityLogDto>> GetByDateRangeAsync(
        DateTime start,
        DateTime end,
        CancellationToken ct = default)
    {
        return await _context.ActivityLogs
            .Where(e => e.CreatedAt >= start && e.CreatedAt <= end)
            .Select(e => new ActivityLogDto(e.EventType, e.Payload, e.CreatedAt))
            .ToListAsync(ct);
    }
}
