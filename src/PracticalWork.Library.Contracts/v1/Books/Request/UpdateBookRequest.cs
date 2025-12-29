using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Request;

/// <summary>
/// Запрос на обновление данных книги.
/// Используется для изменения основных свойств книги,
/// включая название, авторов, описание, год издания,
/// категорию, статус и путь к обложке.
/// </summary>
/// <param name="Title">Название книги.</param>
/// <param name="Authors">Список авторов книги.</param>
/// <param name="Description">Краткое описание книги.</param>
/// <param name="Year">Год издания книги.</param>
/// <param name="Category">Категория книги.</param>
/// <param name="Status">Статус книги.</param>
/// <param name="CoverImagePath">Путь к файлу обложки книги.</param>
public sealed record UpdateBookRequest(
    string Title,
    IReadOnlyList<string> Authors,
    string Description,
    int Year,
    BookCategory? Category,
    BookStatus? Status,
    string CoverImagePath
) : AbstractBook(Title, Authors, Description, Year);
