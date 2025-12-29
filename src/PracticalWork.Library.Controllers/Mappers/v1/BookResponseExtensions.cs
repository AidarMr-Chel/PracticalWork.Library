using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Contracts.v1.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Mappers.v1
{
    /// <summary>
    /// Методы расширения для преобразования сущностей книг
    /// в объекты ответов API.
    /// </summary>
    public static class BookResponseExtensions
    {
        /// <summary>
        /// Преобразует сущность книги в детализированный ответ.
        /// </summary>
        /// <param name="book">Сущность книги.</param>
        /// <param name="isBorrowed">Флаг, указывающий, находится ли книга в выдаче.</param>
        /// <returns>Объект <see cref="BookDetailsResponse"/> с подробной информацией о книге.</returns>
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

        /// <summary>
        /// Преобразует категорию книги из доменной модели в контракт API.
        /// </summary>
        private static BookCategory MapCategory(PracticalWork.Library.Enums.BookCategory category) =>
            category switch
            {
                PracticalWork.Library.Enums.BookCategory.ScientificBook => BookCategory.ScientificBook,
                PracticalWork.Library.Enums.BookCategory.EducationalBook => BookCategory.EducationalBook,
                PracticalWork.Library.Enums.BookCategory.FictionBook => BookCategory.FictionBook,
                _ => BookCategory.Default
            };

        /// <summary>
        /// Преобразует статус книги из доменной модели в контракт API.
        /// </summary>
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
