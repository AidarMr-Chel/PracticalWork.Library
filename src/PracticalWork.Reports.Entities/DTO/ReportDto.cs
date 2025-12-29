namespace PracticalWork.Reports.Entities.DTO;

/// <summary>
/// DTO для отчета
/// </summary>
public sealed class ReportDto
{
    /// <summary>
    /// Идентификатор отчета
    /// </summary>
    public Guid Id { get; set; }

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
}
