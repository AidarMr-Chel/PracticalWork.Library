using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Models;

/// <summary>
/// Модель книги в библиотечной системе.
/// Содержит основную информацию о книге, её статусе и возможных операциях.
/// </summary>
public sealed class Book
{
    /// <summary>
    /// Уникальный идентификатор книги.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название книги.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Список авторов книги.
    /// </summary>
    public IReadOnlyList<string> Authors { get; set; }

    /// <summary>
    /// Описание книги.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Год издания книги.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Категория книги.
    /// </summary>
    public BookCategory Category { get; set; }

    /// <summary>
    /// Текущий статус книги.
    /// </summary>
    public BookStatus Status { get; set; }

    /// <summary>
    /// Путь к файлу обложки книги.
    /// </summary>
    public string CoverImagePath { get; set; }

    /// <summary>
    /// Признак того, что книга заархивирована.
    /// </summary>
    public bool IsArchived { get; set; }

    /// <summary>
    /// Проверяет, может ли книга быть заархивирована.
    /// </summary>
    /// <returns><c>true</c>, если книга не выдана; иначе <c>false</c>.</returns>
    public bool CanBeArchived() => Status != BookStatus.Borrow;

    /// <summary>
    /// Проверяет, доступна ли книга для выдачи.
    /// </summary>
    /// <returns><c>true</c>, если книга не в архиве и имеет статус <see cref="BookStatus.Available"/>.</returns>
    public bool CanBeBorrowed() => !IsArchived && Status == BookStatus.Available;

    /// <summary>
    /// Переводит книгу в архив.
    /// </summary>
    /// <exception cref="InvalidOperationException">Выбрасывается, если книга не может быть заархивирована.</exception>
    public void Archive()
    {
        if (!CanBeArchived())
            throw new InvalidOperationException("Книга не может быть заархивирована.");

        IsArchived = true;
        Status = BookStatus.Archived;
    }

    /// <summary>
    /// Обновляет описание и путь к обложке книги.
    /// </summary>
    /// <param name="description">Новое описание книги.</param>
    /// <param name="coverImagePath">Новый путь к файлу обложки.</param>
    public void UpdateDetails(string description, string coverImagePath)
    {
        Description = description;
        CoverImagePath = coverImagePath;
    }
}
