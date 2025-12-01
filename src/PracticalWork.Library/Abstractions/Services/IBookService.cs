using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services;

public interface IBookService
{
    /// <summary>
    /// Создание книги
    /// </summary>
    Task<Guid> CreateBook(Book book);

    /// <summary>
    /// Обновление книги по идентификатору
    /// </summary>
    Task UpdateBook(Guid id, Book book);

    /// <summary>
    /// Архивирование книги по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Book> ArchivingBook(Guid id);

    Task<IEnumerable<Book>> GetBooks(Book book);
}