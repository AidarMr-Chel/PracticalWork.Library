namespace PracticalWork.Library.Models;

/// <summary>
/// Агрегированная статистика еженедельного отчёта.
/// </summary>
public sealed record WeeklyStats(
    int NewBorrowsCount,
    int ReturnsCount,
    int OverdueCount);
