using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.v1.Events.Borrows;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

/// <summary>
/// Бизнес‑воркфлоу генерации и отправки еженедельного отчёта администрации.
/// Инкапсулирует всю логику сценария, отделённую от Quartz.
/// </summary>
public sealed class WeeklyReportWorkflow : IWorkflow
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IMinioService _minioService;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;
    private readonly MinioReportsSettings _reportsSettings;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<WeeklyReportWorkflow> _logger;

    public WeeklyReportWorkflow(
        IActivityLogRepository activityLogRepository,
        IMinioService minioService,
        IEmailService emailService,
        IEmailTemplateService templateService,
        IOptions<MinioReportsSettings> reportsSettings,
        TimeProvider timeProvider,
        ILogger<WeeklyReportWorkflow> logger)
    {
        _activityLogRepository = activityLogRepository;
        _minioService = minioService;
        _emailService = emailService;
        _templateService = templateService;
        _reportsSettings = reportsSettings.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    /// <summary>
    /// Выполняет генерацию отчёта, загрузку в MinIO и отправку письма администраторам.
    /// </summary>
    public async Task<WorkflowResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("WeeklyReportWorkflow: запуск генерации отчёта");

        var (lastWeekStart, lastWeekEnd) = CalculateLastWeekRange();

        try
        {
            _logger.LogInformation(
                "Период отчёта: {Start} — {End}",
                lastWeekStart.ToString("yyyy-MM-dd"),
                lastWeekEnd.ToString("yyyy-MM-dd"));


            var events = await _activityLogRepository.GetByDateRangeAsync(
                lastWeekStart, lastWeekEnd, cancellationToken);

            _logger.LogInformation("Найдено событий в БД: {Count}", events.Count());

            if (!events.Any())
            {
                _logger.LogWarning("Нет данных для отчёта за неделю {Week}", lastWeekStart);
                return new WorkflowResult(true, 0, 0);
            }

            var stats = AggregateStatistics(events, lastWeekStart, lastWeekEnd);

            var csvBytes = GenerateCsv(stats, lastWeekStart, lastWeekEnd);
            var fileName = $"weekly_report_{lastWeekStart:yyyy-MM-dd}.csv";

            using var csvStream = new MemoryStream(csvBytes);

            await _minioService.UploadToBucketAsync(
                csvStream,
                fileName,
                "text/csv; charset=utf-8",
                _reportsSettings.Bucket);

            _logger.LogInformation("Файл загружен в бакет '{Bucket}': {FileName}",
                _reportsSettings.Bucket, fileName);

            var expiry = TimeSpan.FromSeconds(_reportsSettings.PresignedUrlExpirySeconds);
            var fileUrl = await _minioService.GetPresignedUrlAsync(
                fileName,
                expiry,
                _reportsSettings.Bucket);

            _logger.LogInformation("Presigned‑ссылка (действует {Expiry}): {Url}", expiry, fileUrl);

            var subject = $"Еженедельный отчёт ({lastWeekStart:dd.MM} - {lastWeekEnd.AddDays(1):dd.MM})";
            var emailParams = BuildReportParameters(stats, fileUrl, lastWeekStart, lastWeekEnd, expiry.Days);

            var htmlBody = await _templateService.RenderAsync("WeeklyReport.html", emailParams);
            var textBody = await _templateService.RenderAsync("WeeklyReport.txt", emailParams);

            var sendResults = await _emailService.SendToAdminsAsync(subject, htmlBody, textBody);

            var successCount = sendResults.Count(r => r.IsSuccess);
            var errorCount = sendResults.Count(r => !r.IsSuccess);

            return new WorkflowResult(errorCount == 0, successCount, errorCount);
        }
        catch (AggregateException aggEx)
        {
            _logger.LogError(aggEx, "Ошибки при агрегации статистики отчёта");
            await SendAggregationAlertAsync(aggEx, lastWeekStart, lastWeekEnd);

            return new WorkflowResult(
                false,
                0,
                aggEx.InnerExceptions.Count,
                aggEx.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Критическая ошибка в WeeklyReportWorkflow");
            return new WorkflowResult(false, 0, 1, ex.Message);
        }
    }

    /// <summary>
    /// Вычисляет диапазон дат прошлой недели (UTC).
    /// </summary>
    private (DateTime Start, DateTime End) CalculateLastWeekRange()
    {
        var today = _timeProvider.GetUtcNow().Date;
        var dayOfWeek = (int)today.DayOfWeek;
        var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1;

        var thisMonday = today.AddDays(-daysToSubtract);
        var lastWeekStart = thisMonday.AddDays(-7);
        var lastWeekEnd = lastWeekStart.AddDays(7).AddTicks(-1);

        return (DateTime.SpecifyKind(lastWeekStart, DateTimeKind.Utc),
                DateTime.SpecifyKind(lastWeekEnd, DateTimeKind.Utc));
    }

    /// <summary>
    /// Агрегирует статистику по событиям.
    /// </summary>
    private WeeklyStats AggregateStatistics(IEnumerable<ActivityLogDto> events, DateTime start, DateTime end)
    {
        int newBorrows = 0, returns = 0, overdue = 0;
        var errors = new List<ReportAggregationException>();

        foreach (var e in events)
        {
            try
            {
                switch (e.EventType)
                {
                    case "book.borrowed":
                        var borrowed = JsonSerializer.Deserialize<BookBorrowedEvent>(e.Payload);
                        if (borrowed?.BorrowedAt == null)
                            throw new ReportAggregationException(e.EventType, e.Payload, "BorrowedAt is null");
                        if (borrowed.BorrowedAt >= start && borrowed.BorrowedAt <= end)
                            newBorrows++;
                        break;

                    case "book.returned":
                        var returned = JsonSerializer.Deserialize<BookReturnedEvent>(e.Payload);
                        if (returned?.ReturnedAt == null)
                            throw new ReportAggregationException(e.EventType, e.Payload, "ReturnedAt is null");
                        if (returned.ReturnedAt >= start && returned.ReturnedAt <= end)
                            returns++;
                        break;

                    default:
                        _logger.LogWarning("Неизвестный тип события: {EventType}", e.EventType);
                        break;
                }
            }
            catch (JsonException jsonEx)
            {
                errors.Add(new ReportAggregationException(e.EventType, e.Payload, "JSON deserialization failed", jsonEx));
            }
            catch (ReportAggregationException validationEx)
            {
                errors.Add(validationEx);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Unexpected error aggregating event {EventType}", e.EventType);
                errors.Add(new ReportAggregationException(e.EventType, e.Payload, "Unexpected aggregation error", ex));
            }
        }

        if (errors.Any())
            throw new AggregateException(
                $"Failed to aggregate {errors.Count} out of {events.Count()} events",
                errors);

        return new WeeklyStats(newBorrows, returns, overdue);
    }

    /// <summary>
    /// Формирует параметры для шаблона отчёта.
    /// </summary>
    private Dictionary<string, string> BuildReportParameters(
        WeeklyStats stats, string fileUrl, DateTime start, DateTime end, int expiryDays)
    {
        return new Dictionary<string, string>
        {
            ["PeriodStart"] = start.ToString("dd.MM.yyyy"),
            ["PeriodEnd"] = end.AddDays(1).ToString("dd.MM.yyyy"),
            ["NewBorrows"] = stats.NewBorrowsCount.ToString(),
            ["Returns"] = stats.ReturnsCount.ToString(),
            ["Overdue"] = stats.OverdueCount.ToString(),
            ["DownloadUrl"] = fileUrl,
            ["ExpiryDays"] = expiryDays.ToString()
        };
    }

    /// <summary>
    /// Генерирует CSV‑файл (UTF‑8 + BOM).
    /// </summary>
    private static byte[] GenerateCsv(WeeklyStats stats, DateTime start, DateTime end)
    {
        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var lines = new[]
        {
            "Параметр;Значение",
            $"Период;{start:dd.MM.yyyy} - {end.AddDays(1):dd.MM.yyyy}",
            $"Новых выдач;{stats.NewBorrowsCount}",
            $"Возвратов;{stats.ReturnsCount}",
            $"Просроченных возвратов;{stats.OverdueCount}"
        };

        return bom.Concat(Encoding.UTF8.GetBytes(string.Join("\n", lines))).ToArray();
    }

    /// <summary>
    /// Отправляет письмо‑алерт об ошибке агрегации.
    /// </summary>
    private async Task SendAggregationAlertAsync(AggregateException aggEx, DateTime start, DateTime end)
    {
        var details = string.Join("\n\n", aggEx.InnerExceptions.Select((ex, i) =>
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
            ["PeriodStart"] = start.ToString("yyyy-MM-dd"),
            ["PeriodEnd"] = end.ToString("yyyy-MM-dd"),
            ["ErrorCount"] = aggEx.InnerExceptions.Count.ToString(),
            ["ErrorDetails"] = WebUtility.HtmlEncode(details)
        };

        var subject = $"Ошибка генерации отчёта ({start:dd.MM} - {end:dd.MM})";
        var htmlBody = await _templateService.RenderAsync("AggregationAlert.html", alertParams);
        var textBody = await _templateService.RenderAsync("AggregationAlert.txt", alertParams);

        await _emailService.SendToAdminsAsync(subject, htmlBody, textBody);
        _logger.LogWarning("Отправлен алерт об ошибке агрегации администраторам");
    }
}

/// <summary>
/// Неизменяемая модель статистики.
/// </summary>
public sealed record WeeklyStats(
    int NewBorrowsCount,
    int ReturnsCount,
    int OverdueCount);
