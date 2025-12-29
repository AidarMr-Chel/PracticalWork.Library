namespace PracticalWork.Reports.Entities.DTO;

/// <summary>
/// DTO для запроса на генерацию отчета
/// </summary>
public sealed class GenerateReportRequest
{
    /// <summary>
    /// Дата начала периода отчета
    /// </summary>
    public DateOnly From { get; set; }

    /// <summary>
    /// Дата окончания периода отчета
    /// </summary>
    public DateOnly To { get; set; }

    /// <summary>
    /// Тип события для фильтрации
    /// </summary>
    public string? EventType { get; set; }
}
