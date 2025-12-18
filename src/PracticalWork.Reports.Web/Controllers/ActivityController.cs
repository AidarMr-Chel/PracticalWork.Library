using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Data.PostgreSql;
using PracticalWork.Reports.Entities.DTO;

namespace PracticalWork.Reports.Web.Controllers;

/// <summary>
/// Контроллер для получения логов активности системы.
/// </summary>
[ApiController]
[Route("api/reports/activity")]
public class ActivityController : ControllerBase
{
    private readonly ReportsDbContext _db;

    public ActivityController(ReportsDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Получение логов активности с фильтрацией и пагинацией.
    /// </summary>
    /// <param name="filter">
    /// Фильтры:
    /// <br/>• <b>From</b> — дата начала периода
    /// <br/>• <b>To</b> — дата окончания периода
    /// <br/>• <b>EventType</b> — тип события
    /// <br/>• <b>Page</b> — номер страницы (по умолчанию 1)
    /// <br/>• <b>PageSize</b> — размер страницы (по умолчанию 20)
    /// </param>
    /// <returns>
    /// Объект <see cref="PagedResult{ActivityLogDto}"/> содержащий:
    /// <br/>• список логов
    /// <br/>• общее количество записей
    /// <br/>• номер страницы
    /// <br/>• размер страницы
    /// </returns>
    /// <response code="200">Успешное получение логов активности</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ActivityLogDto>), StatusCodes.Status200OK)]
    public async Task<PagedResult<ActivityLogDto>> Get([FromQuery] ActivityLogFilterDto filter)
    {
        var query = _db.ActivityLogs.AsQueryable();

        if (filter.From.HasValue)
            query = query.Where(x => x.CreatedAt >= filter.From.Value);

        if (filter.To.HasValue)
            query = query.Where(x => x.CreatedAt <= filter.To.Value);

        if (!string.IsNullOrWhiteSpace(filter.EventType))
            query = query.Where(x => x.EventType == filter.EventType);

        var total = await query.CountAsync();

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
            .ToListAsync();

        return new PagedResult<ActivityLogDto>
        {
            Items = items,
            Total = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }
}
