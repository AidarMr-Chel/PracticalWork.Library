using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services;
using PracticalWork.Library.Services.Reports;

namespace PracticalWork.Library.Tests.Services;

public class WeeklyReportWorkflowTests
{
    [Fact]
    public async Task ExecuteAsync_WhenNoEvents_ReturnsSuccessWithoutPublishing()
    {
        var periodStart = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc);
        var periodEnd = new DateTime(2026, 5, 7, 23, 59, 59, DateTimeKind.Utc);

        var activityLog = new Mock<IActivityLogRepository>();
        activityLog.Setup(r => r.GetByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), default))
            .ReturnsAsync(Array.Empty<ActivityLogDto>());

        var period = new Mock<IWeeklyReportPeriodProvider>();
        period.Setup(p => p.GetLastWeekRangeUtc()).Returns((periodStart, periodEnd));

        var filePublisher = new Mock<IWeeklyReportFilePublisher>();
        var email = new Mock<IWeeklyReportEmailNotifier>();

        var sut = new WeeklyReportWorkflow(
            activityLog.Object,
            period.Object,
            new WeeklyReportStatisticsAggregator(NullLogger<WeeklyReportStatisticsAggregator>.Instance),
            new WeeklyReportCsvBuilder(),
            filePublisher.Object,
            email.Object,
            Options.Create(new MinioReportsSettings { Bucket = "reports", PresignedUrlExpirySeconds = 3600 }),
            NullLogger<WeeklyReportWorkflow>.Instance);

        var result = await sut.ExecuteAsync();

        result.IsSuccess.Should().BeTrue();
        filePublisher.Verify(f => f.PublishAsync(It.IsAny<byte[]>(), It.IsAny<string>(), default), Times.Never);
    }
}
