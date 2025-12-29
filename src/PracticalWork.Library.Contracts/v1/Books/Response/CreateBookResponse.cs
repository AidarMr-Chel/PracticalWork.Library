namespace PracticalWork.Library.Contracts.v1.Books.Response;

/// <summary>
/// Ответ на операцию создания книги.
/// Содержит идентификатор созданной книги и её название.
/// </summary>
/// <param name="Id">Идентификатор книги.</param>
/// <param name="Title">Название книги.</param>
public sealed record CreateBookResponse(
    Guid Id,
    string Title
);
