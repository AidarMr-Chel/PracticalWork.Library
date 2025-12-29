using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Models;

/// <summary>
/// Книга
/// </summary>
public sealed class Book
{
    /// <summary>
    /// Идентификатор книги
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Название книги
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Авторы книги
    /// </summary>
    public IReadOnlyList<string> Authors { get; set; }
    /// <summary>
    /// Описание книги
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Год издания
    /// </summary>
    public int Year { get; set; }
    /// <summary>
    /// Категория книги
    /// </summary>
    public BookCategory Category { get; set; }
    /// <summary>
    /// Статус книги
    /// </summary>
    public BookStatus Status { get; set; }
    /// <summary>
    /// Путь к обложке книги
    /// </summary>
    public string CoverImagePath { get; set; }
    /// <summary>
    /// Флаг архивирования книги
    /// </summary>
    public bool IsArchived { get; set; }
    /// <summary>
    /// Проверка возможности архивирования книги
    /// </summary>
    /// <returns></returns>
    public bool CanBeArchived() => Status != BookStatus.Borrow;
    /// <summary>
    /// Проверка возможности взятия книги в аренду
    /// </summary>
    /// <returns></returns>
    public bool CanBeBorrowed() => !IsArchived && Status == BookStatus.Available;
    /// <summary>
    /// Архивирование книги
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Archive()
    {
        if (!CanBeArchived())
            throw new InvalidOperationException("Книга не может быть заархивирована.");
        IsArchived = true;
        Status = BookStatus.Archived;
    }
    /// <summary>
    /// Обновление деталей книги
    /// </summary>
    /// <param name="description"></param>
    /// <param name="coverImagePath"></param>
    public void UpdateDetails(string description, string coverImagePath)
    {
        Description = description;
        CoverImagePath = coverImagePath;
    }
}
