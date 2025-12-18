namespace PracticalWork.Reports.Entities.DTO;

public sealed class ReportDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public DateTime GeneratedAt { get; set; }
}
