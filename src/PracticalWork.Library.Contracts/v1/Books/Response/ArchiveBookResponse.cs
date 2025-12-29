namespace PracticalWork.Library.Contracts.v1.Books.Response;

/// <summary>
/// Ответ на операцию архивирования книги.
/// Содержит идентификатор книги, её название
/// и дату перевода в архив.
/// </summary>
/// <param name="Id">Идентификатор книги.</param>
/// <param name="Title">Название книги.</param>
/// <param name="ArchivedAt">Дата и время архивирования книги.</param>
public sealed record ArchiveBookResponse(
    Guid Id,
    string Title,
    DateTime ArchivedAt
);
