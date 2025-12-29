using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Data.PostgreSql.Entities;

/// <summary>
/// Базовый класс для сущностей книг.
/// Содержит общие поля, используемые всеми типами книг.
/// </summary>
public abstract class AbstractBookEntity : EntityBase
{
    /// <summary>
    /// Название книги.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Список авторов книги.
    /// </summary>
    public IReadOnlyList<string> Authors { get; set; }

    /// <summary>
    /// Краткое описание или аннотация книги.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Год издания книги.
    /// </summary>
    public int Year { get; set; }

    /// <summary>
    /// Текущий статус книги (например, доступна, выдана, утеряна).
    /// </summary>
    public BookStatus Status { get; set; }

    /// <summary>
    /// Путь к изображению обложки книги в файловом хранилище.
    /// </summary>
    public string CoverImagePath { get; set; }

    /// <summary>
    /// Записи о выдаче книги пользователям.
    /// </summary>
    public ICollection<BookBorrowEntity> IssuanceRecords { get; set; }
}
