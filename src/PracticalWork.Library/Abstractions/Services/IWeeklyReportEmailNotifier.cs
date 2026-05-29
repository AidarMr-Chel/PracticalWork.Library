using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Отправляет еженедельный отчёт и уведомления об ошибках агрегации.
/// </summary>
public interface IWeeklyReportEmailNotifier
{
    Task<(int SuccessCount, int ErrorCount)> SendReportAsync(
        WeeklyStats stats,
        string fileUrl,
        DateTime periodStart,
        DateTime periodEnd,
        int urlExpiryDays,
        CancellationToken cancellationToken = default);

    Task SendAggregationFailureAlertAsync(
        AggregateException exception,
        DateTime periodStart,
        DateTime periodEnd,
        CancellationToken cancellationToken = default);
}
