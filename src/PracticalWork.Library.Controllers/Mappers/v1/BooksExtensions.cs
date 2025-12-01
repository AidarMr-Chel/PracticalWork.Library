using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Mappers.v1;

public static class BooksExtensions
{
    public static Book ToBook(this CreateBookRequest request) =>
        new()
        {
            Authors = request.Authors,
            Title = request.Title,
            Description = request.Description,
            Year = request.Year,
            Category = (BookCategory)request.Category
        };

    public static Book ToBook(this PracticalWork.Library.Contracts.v1.Books.Request.UpdateBookRequest request) =>
        new()
        {
            Authors = request.Authors,
            Title = request.Title,
            Description = request.Description,
            Year = request.Year
            // Note: UpdateBookRequest doesn't contain Category — repository will preserve existing category
        };
    public static Book ToBook(this PracticalWork.Library.Contracts.v1.Books.Request.BookFilterRequest request) =>
        new()
        {
            Status = (BookStatus)request.Status,
            Category = (BookCategory)request.Category,
            Authors = request.Authors,
        };
}