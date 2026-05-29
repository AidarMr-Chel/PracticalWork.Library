using FluentAssertions;
using Moq;
using PracticalWork.Reports.Entities;
using PracticalWork.Reports.Entities.Abstractions;
using PracticalWork.Reports.Entities.DTO;
using PracticalWork.Reports.Minio;
using PracticalWork.Reports.Services;

namespace PracticalWork.Reports.Tests.Services;

public class ReportServiceTests
{
    private readonly Mock<IActivityLogRepository> _activityRepo = new();
    private readonly Mock<IReportRepository> _reportRepo = new();
    private readonly Mock<IMinioService> _minio = new();
    private readonly Mock<IReportsCacheService> _cache = new();

    private ReportService CreateSut() =>
        new(_activityRepo.Object, _reportRepo.Object, _minio.Object, _cache.Object);

    [Fact]
    public async Task GenerateReportAsync_UploadsToMinio_SavesReport_InvalidatesCache()
    {
        var request = new GenerateReportRequest
        {
            From = new DateOnly(2026, 5, 1),
            To = new DateOnly(2026, 5, 7)
        };

        _activityRepo.Setup(r => r.GetLogsAsync(request.From, request.To, null, default))
            .ReturnsAsync(new List<ActivityLog>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    EventType = "book.borrowed",
                    Payload = "{}",
                    CreatedAt = DateTime.UtcNow
                }
            });

        _minio.Setup(m => m.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), "text/csv"))
            .ReturnsAsync("path");

        var result = await CreateSut().GenerateReportAsync(request);

        result.Name.Should().StartWith("report_");
        _reportRepo.Verify(r => r.AddAsync(It.IsAny<Report>(), default), Times.Once);
        _cache.Verify(c => c.RemoveAsync("reports:list", default), Times.Once);
        _minio.Verify(m => m.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), "text/csv"), Times.Once);
    }

    [Fact]
    public async Task GetReportsAsync_WhenCached_ReturnsWithoutMinio()
    {
        var cached = new List<ReportInfoDto>
        {
            new() { Name = "report.csv", FilePath = "reports/report.csv", CreatedAt = DateTime.UtcNow }
        };

        _cache.Setup(c => c.GetAsync<List<ReportInfoDto>>("reports:list", default))
            .ReturnsAsync(cached);

        var result = await CreateSut().GetReportsAsync();

        result.Should().BeSameAs(cached);
        _minio.Verify(m => m.ListAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetReportsAsync_WhenNotCached_LoadsFromMinio_AndCaches()
    {
        _cache.Setup(c => c.GetAsync<List<ReportInfoDto>>("reports:list", default))
            .ReturnsAsync((List<ReportInfoDto>)null!);

        _minio.Setup(m => m.ListAsync("reports"))
            .ReturnsAsync(new List<MinioObjectInfo>
            {
                new() { Key = "reports/a.csv", LastModifiedDate = DateTime.UtcNow }
            });

        var result = await CreateSut().GetReportsAsync();

        result.Should().HaveCount(1);
        _cache.Verify(c => c.SetAsync("reports:list", It.IsAny<List<ReportInfoDto>>(), It.IsAny<TimeSpan>(), default), Times.Once);
    }
}
