using PracticalWork.Reports.Entities;

namespace PracticalWork.Reports.Data.PostgreSql.Repositories;

public class ReportRepository
{
    private readonly ReportsDbContext _db;

    public ReportRepository(ReportsDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Report report)
    {
        _db.Reports.Add(report);
        await _db.SaveChangesAsync();
    }
}
