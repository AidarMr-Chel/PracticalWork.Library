using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services.Reports;

namespace PracticalWork.Library.Tests.Services.Reports;

public class WeeklyReportEmailNotifierTests
{
    private readonly Mock<IEmailService> _email = new();
    private readonly Mock<IEmailTemplateService> _templates = new();
    private readonly Mock<IWeeklyReportCsvBuilder> _csvBuilder = new();

    [Fact]
    public async Task SendReportAsync_RendersTemplatesAndReturnsSendCounts()
    {
        var stats = new WeeklyStats(2, 1, 0);
        var start = new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 5, 10, 23, 59, 59, DateTimeKind.Utc);
        var parameters = new Dictionary<string, string> { ["NewBorrows"] = "2" };

        _csvBuilder.Setup(b => b.BuildEmailParameters(stats, "http://report", start, end, 7))
            .Returns(parameters);
        _templates.Setup(t => t.RenderAsync("WeeklyReport.html", parameters))
            .ReturnsAsync("<html>report</html>");
        _templates.Setup(t => t.RenderAsync("WeeklyReport.txt", parameters))
            .ReturnsAsync("report text");
        _email.Setup(e => e.SendToAdminsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new[]
            {
                EmailSendResult.Success(),
                EmailSendResult.Failure("smtp error")
            });

        var (success, errors) = await CreateSut().SendReportAsync(stats, "http://report", start, end, 7);

        success.Should().Be(1);
        errors.Should().Be(1);
        _templates.Verify(t => t.RenderAsync("WeeklyReport.html", parameters), Times.Once);
        _templates.Verify(t => t.RenderAsync("WeeklyReport.txt", parameters), Times.Once);
        _email.Verify(e => e.SendToAdminsAsync(
            It.Is<string>(s => s.Contains("04.05") && s.Contains("11.05")),
            "<html>report</html>",
            "report text"), Times.Once);
    }

    [Fact]
    public async Task SendAggregationFailureAlertAsync_SendsAlertToAdmins()
    {
        var start = new DateTime(2026, 5, 4, 0, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2026, 5, 10, 23, 59, 59, DateTimeKind.Utc);
        var aggregate = new AggregateException(
            new ReportAggregationException("book.borrowed", "{\"id\":1}", "parse error"));

        _templates.Setup(t => t.RenderAsync(It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync("body");
        _email.Setup(e => e.SendToAdminsAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(new[] { EmailSendResult.Success() });

        await CreateSut().SendAggregationFailureAlertAsync(aggregate, start, end);

        _templates.Verify(t => t.RenderAsync("AggregationAlert.html", It.IsAny<Dictionary<string, string>>()), Times.Once);
        _templates.Verify(t => t.RenderAsync("AggregationAlert.txt", It.IsAny<Dictionary<string, string>>()), Times.Once);
        _email.Verify(e => e.SendToAdminsAsync(
            It.Is<string>(s => s.Contains("Ошибка генерации")),
            "body",
            "body"), Times.Once);
    }

    private WeeklyReportEmailNotifier CreateSut() =>
        new(
            _email.Object,
            _templates.Object,
            _csvBuilder.Object,
            NullLogger<WeeklyReportEmailNotifier>.Instance);
}
