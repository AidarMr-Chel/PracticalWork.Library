using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Mappers.v1;

public static class BooksExtensions
{
    public static Book ToBook(this CreateBookRequest request) =>
        new()
        {
            Authors = request.Authors ?? new List<string>(),
            Title = request.Title,
            Description = request.Description,
            Year = request.Year,
            Category = request.Category.HasValue
                ? (BookCategory)request.Category.Value
                : BookCategory.Default,
            Status = BookStatus.Available
        };

    public static Book ToBook(this UpdateBookRequest request) =>
        new()
        {
            Title = request.Title,
            Authors = request.Authors ?? new List<string>(),
            Description = request.Description,
            Year = request.Year,
            Category = request.Category.HasValue
                ? (BookCategory)request.Category.Value
                : BookCategory.Default,
            Status = request.Status.HasValue
                ? (BookStatus)request.Status.Value
                : BookStatus.Available,
            CoverImagePath = request.CoverImagePath
        };

    public static Book ToBook(this BookFilterRequest request) =>
        new()
        {
            Status = request.Status.HasValue
                ? (BookStatus)request.Status.Value
                : BookStatus.Available,
            Category = request.Category.HasValue
                ? (BookCategory)request.Category.Value
                : BookCategory.Default,
            Authors = request.Authors ?? new List<string>()
        };
}
