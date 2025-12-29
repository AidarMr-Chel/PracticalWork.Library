namespace PracticalWork.Reports.Entities.DTO;

/// <summary>
/// DTO для информации об отчете
/// </summary>
public class ReportInfoDto
{
    /// <summary>
    /// Идентификатор отчета
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Путь к файлу отчета
    /// </summary>
    public string FilePath { get; set; } = default!;

    /// <summary>
    /// Дата и время создания отчета
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
