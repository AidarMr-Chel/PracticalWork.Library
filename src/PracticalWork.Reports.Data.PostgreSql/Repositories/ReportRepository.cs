using PracticalWork.Reports.Entities;
using PracticalWork.Reports.Entities.Abstractions;

namespace PracticalWork.Reports.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с отчётами.
/// </summary>
public sealed class ReportRepository : IReportRepository
{
    private readonly ReportsDbContext _db;

    public ReportRepository(ReportsDbContext db)
    {
        _db = db;
    }

    /// <inheritdoc />
    public async Task AddAsync(Report report, CancellationToken cancellationToken = default)
    {
        _db.Reports.Add(report);
        await _db.SaveChangesAsync(cancellationToken);
    }
}
