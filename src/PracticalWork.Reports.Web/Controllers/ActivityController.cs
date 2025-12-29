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
    /// Получение логов активности с возможностью фильтрации и пагинации.
    /// </summary>
    /// <remarks>
    /// Если параметры фильтра не указаны, фильтрация не применяется:
    /// <br/>• <b>From</b> — если не задан, нижняя граница даты не используется
    /// <br/>• <b>To</b> — если не задан, верхняя граница даты не используется
    /// <br/>• <b>EventType</b> — если пустой, фильтр по типу события не применяется
    ///
    /// Значения по умолчанию:
    /// <br/>• <b>Page</b> = 1
    /// <br/>• <b>PageSize</b> = 20
    /// </remarks>
    /// <param name="filter">Фильтры для выборки логов активности</param>
    /// <returns>Страница логов активности</returns>
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
