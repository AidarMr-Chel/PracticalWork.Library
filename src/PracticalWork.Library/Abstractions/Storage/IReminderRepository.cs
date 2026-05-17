using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage;

/// <summary>
/// Репозиторий для получения данных, необходимых для напоминаний о возврате книг.
/// </summary>
public interface IReminderRepository
{
    /// <summary>
    /// Возвращает список напоминаний для выдач с указанным сроком возврата.
    /// </summary>
    /// <param name="targetDueDate">Целевая дата возврата.</param>
    /// <param name="ct">Токен отмены.</param>
    Task<IEnumerable<ReminderData>> GetRemindersForDueDateAsync(DateOnly targetDueDate, CancellationToken ct = default);
}