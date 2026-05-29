namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Публикует файл отчёта во внешнее хранилище.
/// </summary>
public interface IWeeklyReportFilePublisher
{
    Task<string> PublishAsync(byte[] csvBytes, string fileName, CancellationToken cancellationToken = default);
}
