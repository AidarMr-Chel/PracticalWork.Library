namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Вычисляет временной диапазон для еженедельного отчёта.
/// </summary>
public interface IWeeklyReportPeriodProvider
{
    (DateTime Start, DateTime End) GetLastWeekRangeUtc();
}
