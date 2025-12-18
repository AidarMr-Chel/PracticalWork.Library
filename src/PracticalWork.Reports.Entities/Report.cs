namespace PracticalWork.Reports.Entities;

public class Report
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Name { get; set; } = default!;
    public string FilePath { get; set; } = default!;

    public DateTime GeneratedAt { get; set; }

    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }

    public ReportStatus Status { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
