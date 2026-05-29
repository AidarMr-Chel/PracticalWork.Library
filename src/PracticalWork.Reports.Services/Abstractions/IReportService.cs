using PracticalWork.Reports.Entities.DTO;

namespace PracticalWork.Reports.Services.Abstractions;

/// <summary>
/// Сервис генерации и выдачи отчётов.
/// </summary>
public interface IReportService
{
    Task<ReportDto> GenerateReportAsync(GenerateReportRequest request, CancellationToken cancellationToken = default);

    Task<List<ReportInfoDto>> GetReportsAsync(CancellationToken cancellationToken = default);

    Task<string> GetDownloadUrlAsync(string reportName, CancellationToken cancellationToken = default);
}
