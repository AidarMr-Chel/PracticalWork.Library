using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Contracts.v1.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Mappers.v1
{
    public static class BookResponseExtensions
    {
        public static BookDetailsResponse ToDetailsResponse(this Book book, bool isBorrowed = false)
        {
            return new BookDetailsResponse(
                Id: book.Id,
                Title: book.Title,
                Category: MapCategory(book.Category),
                Authors: book.Authors,
                Description: book.Description,
                Year: book.Year,
                CoverImageUrl: string.IsNullOrEmpty(book.CoverImagePath)
                    ? string.Empty
                    : $"/images/covers/{book.CoverImagePath}",
                Status: MapStatus(book.Status),
                IsArchived: book.IsArchived,
                IsBorrowed: isBorrowed
            );
        }

        private static BookCategory MapCategory(PracticalWork.Library.Enums.BookCategory category) =>
            category switch
            {
                PracticalWork.Library.Enums.BookCategory.ScientificBook => BookCategory.ScientificBook,
                PracticalWork.Library.Enums.BookCategory.EducationalBook => BookCategory.EducationalBook,
                PracticalWork.Library.Enums.BookCategory.FictionBook => BookCategory.FictionBook,
                _ => BookCategory.Default
            };

        private static BookStatus MapStatus(PracticalWork.Library.Enums.BookStatus status) =>
            status switch
            {
                PracticalWork.Library.Enums.BookStatus.Available => BookStatus.Available,
                PracticalWork.Library.Enums.BookStatus.Borrow => BookStatus.Borrow,
                PracticalWork.Library.Enums.BookStatus.Archived => BookStatus.Archived,
                _ => BookStatus.Available
            };
    }
}
