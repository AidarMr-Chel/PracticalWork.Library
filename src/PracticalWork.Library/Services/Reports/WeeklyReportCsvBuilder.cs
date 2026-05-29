using System.Text;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services.Reports;

/// <summary>
/// Формирует CSV и параметры email-шаблона еженедельного отчёта.
/// </summary>
public sealed class WeeklyReportCsvBuilder : IWeeklyReportCsvBuilder
{
    /// <inheritdoc />
    public byte[] BuildCsv(WeeklyStats stats, DateTime periodStart, DateTime periodEnd)
    {
        var bom = new byte[] { 0xEF, 0xBB, 0xBF };
        var lines = new[]
        {
            "Параметр;Значение",
            $"Период;{periodStart:dd.MM.yyyy} - {periodEnd.AddDays(1):dd.MM.yyyy}",
            $"Новых выдач;{stats.NewBorrowsCount}",
            $"Возвратов;{stats.ReturnsCount}",
            $"Просроченных возвратов;{stats.OverdueCount}"
        };

        return bom.Concat(Encoding.UTF8.GetBytes(string.Join("\n", lines))).ToArray();
    }

    /// <inheritdoc />
    public Dictionary<string, string> BuildEmailParameters(
        WeeklyStats stats,
        string fileUrl,
        DateTime periodStart,
        DateTime periodEnd,
        int expiryDays)
    {
        return new Dictionary<string, string>
        {
            ["PeriodStart"] = periodStart.ToString("dd.MM.yyyy"),
            ["PeriodEnd"] = periodEnd.AddDays(1).ToString("dd.MM.yyyy"),
            ["NewBorrows"] = stats.NewBorrowsCount.ToString(),
            ["Returns"] = stats.ReturnsCount.ToString(),
            ["Overdue"] = stats.OverdueCount.ToString(),
            ["DownloadUrl"] = fileUrl,
            ["ExpiryDays"] = expiryDays.ToString()
        };
    }
}
