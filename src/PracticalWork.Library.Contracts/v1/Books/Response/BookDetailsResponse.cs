using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Response
{
    /// <summary>
    /// Ответ с детальной информацией о книге.
    /// Содержит расширенные сведения, включая категорию, статус,
    /// признак архивности, факт выдачи и ссылку на обложку.
    /// </summary>
    /// <param name="Id">Идентификатор книги.</param>
    /// <param name="Title">Название книги.</param>
    /// <param name="Category">Категория книги.</param>
    /// <param name="Authors">Список авторов книги.</param>
    /// <param name="Description">Описание книги.</param>
    /// <param name="Year">Год издания книги.</param>
    /// <param name="CoverImageUrl">URL обложки книги.</param>
    /// <param name="Status">Текущий статус книги.</param>
    /// <param name="IsArchived">Признак архивности книги.</param>
    /// <param name="IsBorrowed">Признак того, что книга выдана читателю.</param>
    public sealed record BookDetailsResponse(
        Guid Id,
        string Title,
        BookCategory Category,
        IReadOnlyList<string> Authors,
        string Description,
        int Year,
        string CoverImageUrl,
        BookStatus Status,
        bool IsArchived,
        bool IsBorrowed
    ) : AbstractBook(Title, Authors, Description, Year);
}
