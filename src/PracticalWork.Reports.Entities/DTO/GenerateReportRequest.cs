namespace PracticalWork.Reports.Entities.DTO;

public sealed class GenerateReportRequest
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public string? EventType { get; set; }
}
