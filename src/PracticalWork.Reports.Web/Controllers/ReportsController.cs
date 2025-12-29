using Microsoft.AspNetCore.Mvc;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Services;

namespace PracticalWork.Reports.Web.Controllers;

/// <summary>
/// Контроллер для работы с отчётами: генерация, получение списка и скачивание.
/// Предоставляет API для формирования отчётов по активности системы
/// и получения ссылок на их загрузку.
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
    /// Генерирует отчёт по активности системы за указанный период.
    /// </summary>
    /// <param name="request">
    /// Параметры отчёта:
    /// <br/>• <b>From</b> — начало периода
    /// <br/>• <b>To</b> — конец периода
    /// <br/>• <b>EventType</b> — тип события (необязательно)
    /// </param>
    /// <returns>Метаданные созданного отчёта.</returns>
    /// <response code="200">Отчёт успешно создан.</response>
    /// <response code="400">Ошибка валидации параметров.</response>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(ReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> Generate([FromBody] GenerateReportRequest request)
    {
        var result = await _reports.GenerateReportAsync(request);
        return Ok(result);
    }

    /// <summary>
    /// Возвращает список доступных отчётов.
    /// Использует кэширование для ускорения повторных запросов.
    /// </summary>
    /// <returns>Список отчётов с метаданными.</returns>
    /// <response code="200">Список успешно получен.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ReportInfoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReports()
    {
        var result = await _reports.GetReportsAsync();
        return Ok(result);
    }

    /// <summary>
    /// Возвращает временную ссылку для скачивания отчёта.
    /// </summary>
    /// <param name="reportName">Имя файла отчёта (например: <c>report_2025_12_18.csv</c>).</param>
    /// <returns>Подписанный URL для скачивания отчёта.</returns>
    /// <response code="200">Ссылка успешно сгенерирована.</response>
    /// <response code="404">Отчёт не найден.</response>
    [HttpGet("{reportName}/download")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<IActionResult> Download(string reportName)
    {
        var url = await _reports.GetDownloadUrlAsync(reportName);
        return Ok(new { Url = url });
    }
}
