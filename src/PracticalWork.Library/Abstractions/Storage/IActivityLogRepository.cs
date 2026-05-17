namespace PracticalWork.Library.Abstractions.Storage;

using PracticalWork.Library.Models;

/// <summary>
/// Репозиторий для работы с логами событий в отчётной базе данных.
/// Предоставляет методы выборки агрегированных данных для аналитики.
/// </summary>
public interface IActivityLogRepository
{
    /// <summary>
    /// Возвращает события активности за указанный период.
    /// </summary>
    /// <param name="start">Начальная дата диапазона (включительно).</param>
    /// <param name="end">Конечная дата диапазона (включительно).</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>Коллекция DTO логов активности.</returns>
    Task<IEnumerable<ActivityLogDto>> GetByDateRangeAsync(
        DateTime start,
        DateTime end,
        CancellationToken ct = default);
}
