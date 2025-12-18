namespace PracticalWork.Reports.Entities.DTO;

public class ReportInfoDto
{
    public string Name { get; set; } = default!;
    public string FilePath { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
