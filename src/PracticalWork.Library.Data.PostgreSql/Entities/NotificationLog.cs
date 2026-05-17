using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticalWork.Library.Data.PostgreSql.Entities;

/// <summary>
/// Сущность лога уведомлений.
/// Хранит информацию об отправленных напоминаниях и отчётах.
/// </summary>
[Table("notification_logs")]
public class NotificationLogEntity
{
    /// <summary>
    /// Уникальный идентификатор записи.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Идентификатор связанного события выдачи книги.
    /// </summary>
    [Column("borrow_id")]
    public Guid BorrowId { get; set; }

    /// <summary>
    /// Тип уведомления (например: Reminder, Report).
    /// </summary>
    [Column("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время отправки уведомления.
    /// </summary>
    [Column("sent_at")]
    public DateTime SentAt { get; set; }

    /// <summary>
    /// Статус отправки (например: Sent, Failed).
    /// </summary>
    [Column("status")]
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Сообщение об ошибке, если отправка не удалась.
    /// </summary>
    [Column("error_message")]
    public string ErrorMessage { get; set; }
}
