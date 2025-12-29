using PracticalWork.Library;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage;

public interface IBookRepository
{
    /// <summary>
    /// Добавление книги
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    Task<Guid> AddAsync(Book book);
    /// <summary>
    /// Получение книги по идентификатору
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Book> GetByIdAsync(Guid id);
    /// <summary>
    /// Обновление книги
    /// </summary>
    /// <param name="book"></param>
    /// <returns></returns>
    Task UpdateAsync(Book book);
    /// <summary>
    /// Поиск книг по фильтру
    /// </summary>
    /// <param name="filter"></param>
    /// <returns></returns>
    Task<IEnumerable<Book>> FindAsync(Book filter);
}


