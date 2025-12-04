
public sealed class BorrowDetailsResponse
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid ReaderId { get; set; }
    public DateOnly BorrowDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CoverUrl { get; set; }
}
