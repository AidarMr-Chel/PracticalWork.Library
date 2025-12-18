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
        string CoverImageUrl,
        BookStatus Status,
        bool IsArchived,
        bool IsBorrowed
    ) : AbstractBook(Title, Authors, Description, Year);

}
