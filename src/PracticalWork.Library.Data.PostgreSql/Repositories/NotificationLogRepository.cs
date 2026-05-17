using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с логами уведомлений.
/// Позволяет проверять недавние отправки и добавлять новые записи.
/// </summary>
public sealed class NotificationLogRepository : INotificationLogRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Создаёт экземпляр репозитория логов уведомлений.
    /// </summary>
    /// <param name="context">Контекст основной базы данных приложения.</param>
    public NotificationLogRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Проверяет, было ли уведомление указанного типа отправлено недавно.
    /// </summary>
    /// <param name="borrowId">Идентификатор выдачи книги.</param>
    /// <param name="type">Тип уведомления (например: Reminder, Report).</param>
    /// <param name="timeWindow">Интервал времени, в течение которого проверяется наличие уведомлений.</param>
    /// <returns>True, если уведомление было отправлено в пределах указанного интервала.</returns>
    public async Task<bool> WasNotifiedRecentlyAsync(Guid borrowId, string type, TimeSpan timeWindow)
    {
        var threshold = DateTime.UtcNow - timeWindow;

        return await _context.NotificationLogs
            .AnyAsync(n =>
                n.BorrowId == borrowId &&
                n.Type == type &&
                n.SentAt >= threshold);
    }

    /// <summary>
    /// Добавляет запись о попытке отправки уведомления.
    /// </summary>
    /// <param name="type">Тип уведомления.</param>
    /// <param name="borrowId">Идентификатор выдачи книги.</param>
    /// <param name="status">Статус отправки (Sent или Failed).</param>
    /// <param name="errorMessage">Сообщение об ошибке, если отправка не удалась.</param>
    public async Task AddAsync(string type, Guid borrowId, string status, string errorMessage = null)
    {
        await _context.NotificationLogs.AddAsync(new NotificationLogEntity
        {
            Id = Guid.NewGuid(),
            BorrowId = borrowId,
            Type = type,
            SentAt = DateTime.UtcNow,
            Status = status,
            ErrorMessage = errorMessage
        });
    }

    /// <summary>
    /// Сохраняет изменения в базе данных.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
