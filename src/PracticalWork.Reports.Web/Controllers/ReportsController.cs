using Microsoft.AspNetCore.Mvc;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Services.Abstractions;

namespace PracticalWork.Reports.Web.Controllers;

/// <summary>
/// Контроллер для работы с отчётами.
/// </summary>
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reports;

    public ReportsController(IReportService reports)
    {
        _reports = reports;
    }

    /// <summary>
    /// Генерирует отчёт по активности системы за указанный период.
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Generate([FromBody] GenerateReportRequest request, CancellationToken cancellationToken)
    {
        var result = await _reports.GenerateReportAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает список доступных отчётов.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportInfoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports(CancellationToken cancellationToken)
    {
        var result = await _reports.GetReportsAsync(cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает временную ссылку для скачивания отчёта.
    /// </summary>
    [HttpGet("{reportName}/download")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> Download(string reportName, CancellationToken cancellationToken)
    {
        var url = await _reports.GetDownloadUrlAsync(reportName, cancellationToken);
        return Ok(new { Url = url });
    }
}
