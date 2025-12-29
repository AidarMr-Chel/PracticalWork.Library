using PracticalWork.Library.Contracts.v1.Books.Request;
using PracticalWork.Library.Contracts.v1.Books.Response;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Controllers.Mappers.v1;

/// <summary>
/// Методы расширения для преобразования запросов API
/// в доменную модель книги.
/// </summary>
public static class BooksExtensions
{
    /// <summary>
    /// Преобразует запрос на создание книги в доменную модель <see cref="Book"/>.
    /// </summary>
    /// <param name="request">Запрос на создание книги.</param>
    /// <returns>Экземпляр доменной модели книги.</returns>
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

    /// <summary>
    /// Преобразует запрос на обновление книги в доменную модель <see cref="Book"/>.
    /// </summary>
    /// <param name="request">Запрос на обновление книги.</param>
    /// <returns>Экземпляр доменной модели книги.</returns>
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

    /// <summary>
    /// Преобразует фильтр поиска книг в доменную модель <see cref="Book"/>.
    /// Используется для передачи параметров фильтрации в сервисный слой.
    /// </summary>
    /// <param name="request">Фильтр поиска книг.</param>
    /// <returns>Экземпляр доменной модели книги с заполненными критериями фильтрации.</returns>
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
