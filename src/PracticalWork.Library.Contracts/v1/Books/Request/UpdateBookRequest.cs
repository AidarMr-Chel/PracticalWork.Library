using PracticalWork.Library.Contracts.v1.Abstracts;
using PracticalWork.Library.Contracts.v1.Enums;

namespace PracticalWork.Library.Contracts.v1.Books.Request;

/// <summary>
/// Запрос на обновление книги
/// </summary>
public sealed record UpdateBookRequest(
    string Title,
    IReadOnlyList<string> Authors,
    string Description,
    int Year,
    BookCategory? Category,    
    BookStatus? Status,         
    string CoverImagePath      
) : AbstractBook(Title, Authors, Description, Year);
