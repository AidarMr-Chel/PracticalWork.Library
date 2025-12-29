namespace PracticalWork.Reports.Entities.DTO;

/// <summary>
/// DTO для лога активности
/// </summary>
public class ActivityLogDto
{
    /// <summary>
    /// Идентификатор лога
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Тип события
    /// </summary>
    public string EventType { get; set; } = default!;
    /// <summary>
    /// Полезная нагрузка события
    /// </summary>
    public string Payload { get; set; } = default!;
    /// <summary>
    /// Дата и время создания лога
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
