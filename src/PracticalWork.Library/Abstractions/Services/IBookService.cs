using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис для управления книгами.
/// Определяет операции создания, обновления, архивирования и получения информации о книгах.
/// </summary>
public interface IBookService
{
    /// <summary>
    /// Создаёт новую книгу.
    /// </summary>
    /// <param name="book">Модель книги для создания.</param>
    /// <returns>Идентификатор созданной книги.</returns>
    Task<Guid> CreateBook(Book book);

    /// <summary>
    /// Обновляет данные книги по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор книги.</param>
    /// <param name="book">Обновлённые данные книги.</param>
    Task UpdateBook(Guid id, Book book);

    /// <summary>
    /// Архивирует книгу по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор книги.</param>
    /// <returns>Обновлённая модель книги после архивирования.</returns>
    Task<Book> ArchivingBook(Guid id);

    /// <summary>
    /// Возвращает список книг, удовлетворяющих указанному фильтру.
    /// Поддерживает пагинацию.
    /// </summary>
    /// <param name="filter">Фильтр по свойствам книги.</param>
    /// <param name="pageNumber">Номер страницы (по умолчанию 1).</param>
    /// <param name="pageSize">Размер страницы (по умолчанию 10).</param>
    /// <returns>Коллекция книг, соответствующих фильтру.</returns>
    Task<IEnumerable<Book>> GetBooks(Book filter, int pageNumber = 1, int pageSize = 10);

    /// <summary>
    /// Обновляет дополнительные детали книги:
    /// описание, обложку и связанные метаданные.
    /// </summary>
    /// <param name="bookId">Идентификатор книги.</param>
    /// <param name="description">Описание книги.</param>
    /// <param name="coverStream">Поток данных обложки.</param>
    /// <param name="fileName">Имя файла обложки.</param>
    /// <param name="contentType">MIME‑тип файла обложки.</param>
    Task UpdateBookDetailsAsync(Guid bookId, string description, Stream coverStream, string fileName, string contentType);
}
