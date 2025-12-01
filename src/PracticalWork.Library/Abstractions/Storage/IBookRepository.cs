using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage;

public interface IBookRepository
{
    Task<Guid> CreateBook(Book book);

    /// <summary>
    /// Обновление полей книги по идентификатору
    /// </summary>
    Task UpdateBook(Guid id, Book book);
    Task<Book> ArchivingBook(Guid id);
    Task<IEnumerable<Book>> GetBooks(Book book);
}