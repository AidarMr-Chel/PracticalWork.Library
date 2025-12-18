using System.Text;
using System.Text.Json;
using PracticalWork.Reports.Cache.Redis;
using PracticalWork.Reports.Data.PostgreSql;
using PracticalWork.Reports.Data.PostgreSql.Repositories;
using PracticalWork.Reports.Entities;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Minio;

namespace PracticalWork.Reports.Services;

public class ReportService
{
    private readonly ActivityLogRepository _activityRepo;
    private readonly ReportRepository _reportRepo;
    private readonly IMinioService _minio;
    private readonly RedisCacheService _cache;

    public ReportService(
        ActivityLogRepository activityRepo,
        ReportRepository reportRepo,
        IMinioService minio,
        RedisCacheService cache)
    {
        _activityRepo = activityRepo;
        _reportRepo = reportRepo;
        _minio = minio;
        _cache = cache;
    }

    public async Task<ReportDto> GenerateReportAsync(GenerateReportRequest request)
    {
        var logs = await _activityRepo.GetLogsAsync(request.From, request.To, request.EventType);

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

        await _reportRepo.AddAsync(report);
        await _cache.RemoveAsync("reports:list");

        return new ReportDto
        {
            Id = report.Id,
            Name = report.Name,
            FilePath = report.FilePath,
            GeneratedAt = report.GeneratedAt
        };
    }

    private byte[] GenerateCsv(IEnumerable<ActivityLog> logs)
    {
        var sb = new StringBuilder();
        sb.AppendLine("CreatedAt,EventType,Payload");

        foreach (var log in logs)
        {
            sb.AppendLine($"{log.CreatedAt},{log.EventType},{log.Payload}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }


    public async Task<List<ReportInfoDto>> GetReportsAsync()
    {
        var cached = await _cache.GetAsync<List<ReportInfoDto>>("reports:list");
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

        await _cache.SetAsync("reports:list", reports, TimeSpan.FromHours(24));

        return reports;
    }

    public async Task<string> GetDownloadUrlAsync(string reportName)
    {
        var objects = await _minio.ListAsync("reports");

        var file = objects.FirstOrDefault(o =>
            Path.GetFileName(o.Key).Equals(reportName, StringComparison.OrdinalIgnoreCase));

        if (file == null)
            throw new FileNotFoundException("Report not found");

        return await _minio.GetSignedUrlAsync(file.Key, TimeSpan.FromMinutes(10));
    }


}
