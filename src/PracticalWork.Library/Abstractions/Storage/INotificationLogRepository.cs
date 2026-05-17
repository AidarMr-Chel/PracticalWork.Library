namespace PracticalWork.Library.Abstractions.Storage;

/// <summary>
/// Репозиторий для работы с логами уведомлений.
/// Позволяет проверять недавние отправки и добавлять новые записи.
/// </summary>
public interface INotificationLogRepository
{
    /// <summary>
    /// Проверяет, было ли уведомление указанного типа отправлено недавно.
    /// </summary>
    /// <param name="borrowId">Идентификатор выдачи книги.</param>
    /// <param name="type">Тип уведомления (например: Reminder, Report).</param>
    /// <param name="timeWindow">Интервал времени, в течение которого проверяется наличие уведомлений.</param>
    /// <returns>True, если уведомление было отправлено в пределах указанного интервала.</returns>
    Task<bool> WasNotifiedRecentlyAsync(Guid borrowId, string type, TimeSpan timeWindow);

    /// <summary>
    /// Добавляет запись о попытке отправки уведомления.
    /// </summary>
    /// <param name="type">Тип уведомления.</param>
    /// <param name="borrowId">Идентификатор выдачи книги.</param>
    /// <param name="status">Статус отправки (Sent или Failed).</param>
    /// <param name="errorMessage">Сообщение об ошибке, если отправка не удалась.</param>
    Task AddAsync(string type, Guid borrowId, string status, string errorMessage = null);

    /// <summary>
    /// Сохраняет изменения в базе данных.
    /// </summary>
    Task SaveChangesAsync();
}
