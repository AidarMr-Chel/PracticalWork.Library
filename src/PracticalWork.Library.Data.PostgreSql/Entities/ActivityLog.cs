using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticalWork.Library.Data.PostgreSql.Entities;

/// <summary>
/// Сущность лога активности.
/// Используется для хранения событий системы вместе с полезной нагрузкой.
/// </summary>
[Table("activity_logs")]
public class ActivityLog
{
    /// <summary>
    /// Уникальный идентификатор записи лога.
    /// </summary>
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Тип события (например: WorkflowStarted, WorkflowCompleted, ErrorOccurred).
    /// </summary>
    [Column("event_type")]
    public string EventType { get; set; } = string.Empty;

    /// <summary>
    /// Полезная нагрузка события в сериализованном виде (JSON).
    /// </summary>
    [Column("payload")]
    public string Payload { get; set; } = string.Empty;

    /// <summary>
    /// Дата и время создания записи.
    /// </summary>
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}
