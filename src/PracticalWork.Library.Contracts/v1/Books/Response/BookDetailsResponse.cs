using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Response
{
    /// <summary>
    /// Ответ с детальной информацией по книге
    /// </summary>
    public sealed record BookDetailsResponse(
        Guid Id,
        string Title,
        BookCategory Category,
        IReadOnlyList<string> Authors,
        string Description,
        int Year,
        string CoverImageUrl,   // теперь URL, а не просто путь
        BookStatus Status,
        bool IsArchived,
        bool IsBorrowed          // добавлено для сценария 2
    ) : AbstractBook(Title, Authors, Description, Year);
}
