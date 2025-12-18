using Microsoft.AspNetCore.Mvc;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Services;

namespace PracticalWork.Reports.Web.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    private readonly ReportService _reports;

    public ReportsController(ReportService reports)
    {
        _reports = reports;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateReportRequest request)
    {
        var result = await _reports.GenerateReportAsync(request);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetReports()
    {
        var result = await _reports.GetReportsAsync();
        return Ok(result);
    }

    [HttpGet("{reportName}/download")]
    public async Task<IActionResult> Download(string reportName)
    {
        var url = await _reports.GetDownloadUrlAsync(reportName);
        return Ok(new { Url = url });
    }

}
