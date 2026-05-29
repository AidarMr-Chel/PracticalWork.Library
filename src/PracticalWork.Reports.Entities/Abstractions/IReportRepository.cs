namespace PracticalWork.Reports.Entities.Abstractions;

/// <summary>
/// Порт сохранения отчётов.
/// </summary>
public interface IReportRepository
{
    Task AddAsync(Report report, CancellationToken cancellationToken = default);
}
