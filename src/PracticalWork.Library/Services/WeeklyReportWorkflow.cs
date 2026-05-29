using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

/// <summary>
/// Оркестратор сценария еженедельного отчёта.
/// </summary>
public sealed class WeeklyReportWorkflow : IWorkflow
{
    private readonly IActivityLogRepository _activityLogRepository;
    private readonly IWeeklyReportPeriodProvider _periodProvider;
    private readonly IWeeklyReportStatisticsAggregator _statisticsAggregator;
    private readonly IWeeklyReportCsvBuilder _csvBuilder;
    private readonly IWeeklyReportFilePublisher _filePublisher;
    private readonly IWeeklyReportEmailNotifier _emailNotifier;
    private readonly MinioReportsSettings _reportsSettings;
    private readonly ILogger<WeeklyReportWorkflow> _logger;

    public WeeklyReportWorkflow(
        IActivityLogRepository activityLogRepository,
        IWeeklyReportPeriodProvider periodProvider,
        IWeeklyReportStatisticsAggregator statisticsAggregator,
        IWeeklyReportCsvBuilder csvBuilder,
        IWeeklyReportFilePublisher filePublisher,
        IWeeklyReportEmailNotifier emailNotifier,
        IOptions<MinioReportsSettings> reportsSettings,
        ILogger<WeeklyReportWorkflow> logger)
    {
        _activityLogRepository = activityLogRepository;
        _periodProvider = periodProvider;
        _statisticsAggregator = statisticsAggregator;
        _csvBuilder = csvBuilder;
        _filePublisher = filePublisher;
        _emailNotifier = emailNotifier;
        _reportsSettings = reportsSettings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<WorkflowResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("WeeklyReportWorkflow: запуск генерации отчёта");

        var (lastWeekStart, lastWeekEnd) = _periodProvider.GetLastWeekRangeUtc();

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

            var stats = _statisticsAggregator.Aggregate(events, lastWeekStart, lastWeekEnd);

            var csvBytes = _csvBuilder.BuildCsv(stats, lastWeekStart, lastWeekEnd);
            var fileName = $"weekly_report_{lastWeekStart:yyyy-MM-dd}.csv";

            var fileUrl = await _filePublisher.PublishAsync(csvBytes, fileName, cancellationToken);

            var expiryDays = TimeSpan.FromSeconds(_reportsSettings.PresignedUrlExpirySeconds).Days;
            var (successCount, errorCount) = await _emailNotifier.SendReportAsync(
                stats, fileUrl, lastWeekStart, lastWeekEnd, expiryDays, cancellationToken);

            return new WorkflowResult(errorCount == 0, successCount, errorCount);
        }
        catch (AggregateException aggEx)
        {
            _logger.LogError(aggEx, "Ошибки при агрегации статистики отчёта");
            await _emailNotifier.SendAggregationFailureAlertAsync(aggEx, lastWeekStart, lastWeekEnd, cancellationToken);

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
}
