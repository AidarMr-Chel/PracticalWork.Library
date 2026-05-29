using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services.Reports;

namespace PracticalWork.Library.Tests.Services.Reports;

public class WeeklyReportFilePublisherTests
{
    [Fact]
    public async Task PublishAsync_UploadsCsvAndReturnsPresignedUrl()
    {
        var minio = new Mock<IMinioService>();
        minio.Setup(m => m.UploadToBucketAsync(
                It.IsAny<Stream>(),
                "report.csv",
                "text/csv; charset=utf-8",
                "reports"))
            .ReturnsAsync("reports/report.csv");
        minio.Setup(m => m.GetPresignedUrlAsync("report.csv", It.IsAny<TimeSpan>(), "reports"))
            .ReturnsAsync("https://minio/report.csv");

        var settings = Options.Create(new MinioReportsSettings
        {
            Bucket = "reports",
            PresignedUrlExpirySeconds = 3600
        });

        var sut = new WeeklyReportFilePublisher(
            minio.Object,
            settings,
            NullLogger<WeeklyReportFilePublisher>.Instance);

        var url = await sut.PublishAsync(new byte[] { 1, 2, 3 }, "report.csv");

        url.Should().Be("https://minio/report.csv");
        minio.Verify(m => m.UploadToBucketAsync(
            It.IsAny<Stream>(),
            "report.csv",
            "text/csv; charset=utf-8",
            "reports"), Times.Once);
        minio.Verify(m => m.GetPresignedUrlAsync(
            "report.csv",
            TimeSpan.FromSeconds(3600),
            "reports"), Times.Once);
    }
}
