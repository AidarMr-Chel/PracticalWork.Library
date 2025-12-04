using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Models;

/// <summary>
/// Книга
/// </summary>
public sealed class Book
{
    public Guid Id { get; set; }  
    public string Title { get; set; }
    public IReadOnlyList<string> Authors { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public BookCategory Category { get; set; }
    public BookStatus Status { get; set; }
    public string CoverImagePath { get; set; }
    public bool IsArchived { get; set; }

    public bool CanBeArchived() => Status != BookStatus.Borrow;
    public bool CanBeBorrowed() => !IsArchived && Status == BookStatus.Available;

    public void Archive()
    {
        if (!CanBeArchived())
            throw new InvalidOperationException("Книга не может быть заархивирована.");
        IsArchived = true;
        Status = BookStatus.Archived;
    }

    public void UpdateDetails(string description, string coverImagePath)
    {
        Description = description;
        CoverImagePath = coverImagePath;
    }
}
