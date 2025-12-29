/// <summary>
/// Ответ с деталями выдачи книги
/// </summary>
public sealed class BorrowDetailsResponse
{
    /// <summary>
    /// Идентификатор выдачи
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid BookId { get; set; }
    /// <summary>
    /// Идентификатор читателя
    /// </summary>
    public Guid ReaderId { get; set; }
    /// <summary>
    /// Дата выдачи
    /// </summary>
    public DateOnly BorrowDate { get; set; }
    /// <summary>
    /// Дата возврата
    /// </summary>
    public DateOnly DueDate { get; set; }
    /// <summary>
    /// Дата фактического возврата
    /// </summary>
    public DateOnly? ReturnDate { get; set; }
    /// <summary>
    /// Статус выдачи
    /// </summary>
    public string Status { get; set; } = string.Empty;
    /// <summary>
    /// Название книги
    /// </summary>
    public string CoverUrl { get; set; }
}
