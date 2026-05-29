using System.Text;
using PracticalWork.Reports.Entities;
using PracticalWork.Reports.Entities.Abstractions;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Minio;
using PracticalWork.Reports.Services.Abstractions;

namespace PracticalWork.Reports.Services;

/// <summary>
/// Сервис для работы с отчётами.
/// </summary>
public sealed class ReportService : IReportService
{
    private readonly IActivityLogRepository _activityRepo;
    private readonly IReportRepository _reportRepo;
    private readonly IMinioService _minio;
    private readonly IReportsCacheService _cache;

    public ReportService(
        IActivityLogRepository activityRepo,
        IReportRepository reportRepo,
        IMinioService minio,
        IReportsCacheService cache)
    {
        _activityRepo = activityRepo;
        _reportRepo = reportRepo;
        _minio = minio;
        _cache = cache;
    }

    /// <inheritdoc />
    public async Task<ReportDto> GenerateReportAsync(
        GenerateReportRequest request,
        CancellationToken cancellationToken = default)
    {
        var logs = await _activityRepo.GetLogsAsync(request.From, request.To, request.EventType, cancellationToken);

        var csv = GenerateCsv(logs);

        var fileName = $"report_{Guid.NewGuid():N}.csv";
        var filePath = $"reports/{request.From:yyyy-MM-dd}_{request.To:yyyy-MM-dd}/{fileName}";

        using var stream = new MemoryStream(csv);
        await _minio.UploadAsync(stream, filePath, "text/csv");

        var report = new Report
        {
            Id = Guid.NewGuid(),
            Name = fileName,
            FilePath = filePath,
            GeneratedAt = DateTime.UtcNow,
            PeriodFrom = request.From,
            PeriodTo = request.To,
            Status = ReportStatus.Completed
        };

        await _reportRepo.AddAsync(report, cancellationToken);
        await _cache.RemoveAsync("reports:list", cancellationToken);

        return new ReportDto
        {
            Id = report.Id,
            Name = report.Name,
            FilePath = report.FilePath,
            GeneratedAt = report.GeneratedAt
        };
    }

    /// <inheritdoc />
    public async Task<List<ReportInfoDto>> GetReportsAsync(CancellationToken cancellationToken = default)
    {
        var cached = await _cache.GetAsync<List<ReportInfoDto>>("reports:list", cancellationToken);
        if (cached != null)
            return cached;

        var objects = await _minio.ListAsync("reports");

        var reports = objects
            .Select(o => new ReportInfoDto
            {
                Name = Path.GetFileName(o.Key),
                FilePath = o.Key,
                CreatedAt = o.LastModifiedDate ?? DateTime.UtcNow
            })
            .OrderByDescending(x => x.CreatedAt)
            .ToList();

        await _cache.SetAsync("reports:list", reports, TimeSpan.FromHours(24), cancellationToken);

        return reports;
    }

    /// <inheritdoc />
    public async Task<string> GetDownloadUrlAsync(string reportName, CancellationToken cancellationToken = default)
    {
        var objects = await _minio.ListAsync("reports");

        var file = objects.FirstOrDefault(o =>
            Path.GetFileName(o.Key).Equals(reportName, StringComparison.OrdinalIgnoreCase));

        if (file == null)
            throw new FileNotFoundException("Report not found");

        return await _minio.GetSignedUrlAsync(file.Key, TimeSpan.FromMinutes(10));
    }

    private static byte[] GenerateCsv(IEnumerable<ActivityLog> logs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CreatedAt,EventType,Payload");

        foreach (var log in logs)
            sb.AppendLine($"{log.CreatedAt},{log.EventType},{log.Payload}");

        return Encoding.UTF8.GetBytes(sb.ToString());
    }
}
