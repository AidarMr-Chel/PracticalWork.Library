using Microsoft.AspNetCore.Mvc;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Services.Abstractions;

namespace PracticalWork.Reports.Web.Controllers;

/// <summary>
/// Контроллер для получения логов активности системы.
/// </summary>
[ApiController]
[Route("api/reports/activity")]
public class ActivityController : ControllerBase
{
    private readonly IActivityLogService _activityLogService;

    public ActivityController(IActivityLogService activityLogService)
    {
        _activityLogService = activityLogService;
    }

    /// <summary>
    /// Возвращает страницу логов активности с возможностью фильтрации и пагинации.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ActivityLogDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<ActivityLogDto>> Get([FromQuery] ActivityLogFilterDto filter, CancellationToken cancellationToken)
        => _activityLogService.GetPagedAsync(filter, cancellationToken);
}
