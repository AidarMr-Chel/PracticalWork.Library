using PracticalWork.Reports.Entities;

namespace PracticalWork.Reports.Data.PostgreSql.Repositories;

public class ReportRepository
{
    private readonly ReportsDbContext _db;

    public ReportRepository(ReportsDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Добавляет отчет в хранилище
    /// </summary>
    /// <param name="report"></param>
    /// <returns></returns>
    public async Task AddAsync(Report report)
    {
        _db.Reports.Add(report);
        await _db.SaveChangesAsync();
    }
}
