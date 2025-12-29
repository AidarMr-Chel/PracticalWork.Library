using PracticalWork.Reports.Entities;

namespace PracticalWork.Reports.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с отчётами.
/// Содержит операции сохранения отчётов в хранилище.
/// </summary>
public class ReportRepository
{
    private readonly ReportsDbContext _db;

    public ReportRepository(ReportsDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Добавляет новый отчёт в хранилище и сохраняет изменения.
    /// </summary>
    /// <param name="report">Отчёт, который необходимо сохранить.</param>
    public async Task AddAsync(Report report)
    {
        _db.Reports.Add(report);
        await _db.SaveChangesAsync();
    }
}
