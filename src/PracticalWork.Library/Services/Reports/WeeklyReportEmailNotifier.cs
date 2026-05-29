using System.Net;
using Microsoft.Extensions.Logging;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services.Reports;

/// <summary>
/// Отправляет письма с еженедельным отчётом и алертами об ошибках.
/// </summary>
public sealed class WeeklyReportEmailNotifier : IWeeklyReportEmailNotifier
{
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;
    private readonly IWeeklyReportCsvBuilder _csvBuilder;
    private readonly ILogger<WeeklyReportEmailNotifier> _logger;

    public WeeklyReportEmailNotifier(
        IEmailService emailService,
        IEmailTemplateService templateService,
        IWeeklyReportCsvBuilder csvBuilder,
        ILogger<WeeklyReportEmailNotifier> logger)
    {
        _emailService = emailService;
        _templateService = templateService;
        _csvBuilder = csvBuilder;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<(int SuccessCount, int ErrorCount)> SendReportAsync(
        WeeklyStats stats,
        string fileUrl,
        DateTime periodStart,
        DateTime periodEnd,
        int urlExpiryDays,
        CancellationToken cancellationToken = default)
    {
        var subject = $"Еженедельный отчёт ({periodStart:dd.MM} - {periodEnd.AddDays(1):dd.MM})";
        var emailParams = _csvBuilder.BuildEmailParameters(stats, fileUrl, periodStart, periodEnd, urlExpiryDays);

        var htmlBody = await _templateService.RenderAsync("WeeklyReport.html", emailParams);
        var textBody = await _templateService.RenderAsync("WeeklyReport.txt", emailParams);

        var sendResults = await _emailService.SendToAdminsAsync(subject, htmlBody, textBody);

        return (
            sendResults.Count(r => r.IsSuccess),
            sendResults.Count(r => !r.IsSuccess));
    }

    /// <inheritdoc />
    public async Task SendAggregationFailureAlertAsync(
        AggregateException exception,
        DateTime periodStart,
        DateTime periodEnd,
        CancellationToken cancellationToken = default)
    {
        var details = string.Join("\n\n", exception.InnerExceptions.Select((ex, i) =>
        {
            var reportEx = ex as ReportAggregationException;
            var payloadPreview = reportEx?.Payload != null
                ? reportEx.Payload[..Math.Min(200, reportEx.Payload.Length)] + "..."
                : "N/A";

            return $"[{i + 1}] {ex.GetType().Name}: {ex.Message}\n" +
                   $"EventType: {reportEx?.EventType ?? "N/A"}\n" +
                   $"Payload: {payloadPreview}";
        }));

        var alertParams = new Dictionary<string, string>
        {
            ["PeriodStart"] = periodStart.ToString("yyyy-MM-dd"),
            ["PeriodEnd"] = periodEnd.ToString("yyyy-MM-dd"),
            ["ErrorCount"] = exception.InnerExceptions.Count.ToString(),
            ["ErrorDetails"] = WebUtility.HtmlEncode(details)
        };

        var subject = $"Ошибка генерации отчёта ({periodStart:dd.MM} - {periodEnd:dd.MM})";
        var htmlBody = await _templateService.RenderAsync("AggregationAlert.html", alertParams);
        var textBody = await _templateService.RenderAsync("AggregationAlert.txt", alertParams);

        await _emailService.SendToAdminsAsync(subject, htmlBody, textBody);
        _logger.LogWarning("Отправлен алерт об ошибке агрегации администраторам");
    }
}
