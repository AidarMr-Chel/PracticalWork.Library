using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Агрегирует статистику по событиям активности.
/// </summary>
public interface IWeeklyReportStatisticsAggregator
{
    WeeklyStats Aggregate(
        IEnumerable<ActivityLogDto> events,
        DateTime periodStart,
        DateTime periodEnd);
}
