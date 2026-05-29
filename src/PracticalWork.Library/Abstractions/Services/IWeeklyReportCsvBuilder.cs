using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Формирует CSV и параметры шаблона еженедельного отчёта.
/// </summary>
public interface IWeeklyReportCsvBuilder
{
    byte[] BuildCsv(WeeklyStats stats, DateTime periodStart, DateTime periodEnd);

    Dictionary<string, string> BuildEmailParameters(
        WeeklyStats stats,
        string fileUrl,
        DateTime periodStart,
        DateTime periodEnd,
        int expiryDays);
}
