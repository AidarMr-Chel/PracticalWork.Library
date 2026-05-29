using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Tests.Helpers;

internal static class TestData
{
    public static Book CreateBook(
        Guid? id = null,
        BookStatus status = BookStatus.Available,
        BookCategory category = BookCategory.FictionBook)
    {
        return new Book
        {
            Id = id ?? Guid.NewGuid(),
            Title = "Test Book",
            Authors = new[] { "Author" },
            Description = "Description",
            Year = 2020,
            Category = category,
            Status = status,
            IsArchived = status == BookStatus.Archived
        };
    }

    public static Reader CreateReader(Guid? id = null, bool isActive = true)
    {
        return new Reader
        {
            Id = id ?? Guid.NewGuid(),
            FullName = "Ivan Ivanov",
            PhoneNumber = "+79001234567",
            Email = "ivan@example.com",
            ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1)),
            IsActive = isActive
        };
    }
}
