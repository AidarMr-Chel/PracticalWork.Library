namespace PracticalWork.Library.Contracts.v1.Books.Response;

/// <summary>
/// Ответ на создание книги
/// </summary>
/// <param name="Id">Идентификатор книги</param>
/// <param name="Title"></param>
public sealed record CreateBookResponse(Guid Id, string Title);