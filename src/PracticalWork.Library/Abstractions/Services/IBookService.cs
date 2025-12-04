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

    /// <summary>
    /// Получение списка книг по фильтру
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    Task<IEnumerable<Book>> GetBooks(Book filter, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Добавление деталей о книге
    /// </summary>
    /// <param name="bookId"></param>
    /// <param name="description"></param>
    /// <param name="coverStream"></param>
    /// <param name="fileName"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    Task UpdateBookDetailsAsync(Guid bookId, string description, Stream coverStream, string fileName, string contentType);

}