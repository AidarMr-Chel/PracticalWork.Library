namespace PracticalWork.Reports.Entities.DTO;

/// <summary>
/// DTO для фильтрации логов активности
/// </summary>
public class ActivityLogFilterDto
{
    /// <summary>
    /// Дата и время с которого начинать поиск
    /// </summary>
    public DateTime? From { get; set; }

    /// <summary>
    /// Дата и время по которое искать
    /// </summary>
    public DateTime? To { get; set; }

    /// <summary>
    /// Тип события для фильтрации
    /// </summary>
    public string? EventType { get; set; }

    /// <summary>
    /// Номер страницы для пагинации
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Размер страницы для пагинации
    /// </summary>
    public int PageSize { get; set; } = 20;
}
