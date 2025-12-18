using Microsoft.AspNetCore.Mvc;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Services;

namespace PracticalWork.Reports.Web.Controllers;

/// <summary>
/// Контроллер для работы с отчетами: генерация, получение списка и скачивание.
/// </summary>
[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reports;

    public ReportsController(ReportService reports)
    {
        _reports = reports;
    }

    /// <summary>
    /// Генерация отчета по активности системы.
    /// </summary>
    /// <param name="request">
    /// Параметры отчета:
    /// <br/>• <b>From</b> — начало периода
    /// <br/>• <b>To</b> — конец периода
    /// <br/>• <b>EventType</b> — тип события (опционально)
    /// </param>
    /// <returns>Метаданные созданного отчета.</returns>
    /// <response code="200">Отчет успешно создан</response>
    /// <response code="400">Ошибка валидации параметров</response>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Generate([FromBody] GenerateReportRequest request)
    {
        var result = await _reports.GenerateReportAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Получение списка доступных отчетов.
    /// </summary>
    /// <returns>Список отчетов с метаданными.</returns>
    /// <response code="200">Список успешно получен</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportInfoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports()
    {
        var result = await _reports.GetReportsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Получение ссылки для скачивания отчета.
    /// </summary>
    /// <param name="reportName">Имя файла отчета (например: report_2025_12_18.csv)</param>
    /// <returns>Signed URL для скачивания отчета.</returns>
    /// <response code="200">Ссылка успешно сгенерирована</response>
    /// <response code="404">Отчет не найден</response>
    [HttpGet("{reportName}/download")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> Download(string reportName)
    {
        var url = await _reports.GetDownloadUrlAsync(reportName);
        return Ok(new { Url = url });
    }
}
