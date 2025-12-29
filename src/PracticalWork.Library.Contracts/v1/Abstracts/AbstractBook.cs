namespace PracticalWork.Library.Contracts.v1.Abstracts;

/// <summary>
/// Базовая контрактная модель книги.
/// Используется как основа для DTO, связанных с созданием и обновлением книг.
/// </summary>
/// <param name="Title">Название книги.</param>
/// <param name="Authors">Список авторов книги.</param>
/// <param name="Description">Краткое описание книги.</param>
/// <param name="Year">Год издания книги.</param>
public abstract record AbstractBook(
    string Title,
    IReadOnlyList<string> Authors,
    string Description,
    int Year
);
