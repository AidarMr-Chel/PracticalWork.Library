namespace PracticalWork.Reports.Entities.DTO;

public class ActivityLogFilterDto
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? EventType { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
