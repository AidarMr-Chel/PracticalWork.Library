using PracticalWork.Library.Abstractions.Services;

namespace PracticalWork.Library.Services.Reports;

/// <summary>
/// Вычисляет диапазон прошлой календарной недели (UTC).
/// </summary>
public sealed class WeeklyReportPeriodProvider : IWeeklyReportPeriodProvider
{
    private readonly TimeProvider _timeProvider;

    public WeeklyReportPeriodProvider(TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public (DateTime Start, DateTime End) GetLastWeekRangeUtc()
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
}
