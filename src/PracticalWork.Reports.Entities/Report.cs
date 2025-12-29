namespace PracticalWork.Reports.Entities;

/// <summary>
/// Отчет
/// </summary>
public class Report
{
    /// <summary>
    /// Идентификатор отчета
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Название отчета
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Путь к файлу отчета
    /// </summary>
    public string FilePath { get; set; } = default!;

    /// <summary>
    /// Дата и время генерации отчета
    /// </summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Период, за который сгенерирован отчет
    /// </summary>
    public DateOnly PeriodFrom { get; set; }

    /// <summary>
    /// Период, по который сгенерирован отчет
    /// </summary>
    public DateOnly PeriodTo { get; set; }

    /// <summary>
    /// Статус отчета
    /// </summary>
    public ReportStatus Status { get; set; }

    /// <summary>
    /// Дата и время создания отчета
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления отчета
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
