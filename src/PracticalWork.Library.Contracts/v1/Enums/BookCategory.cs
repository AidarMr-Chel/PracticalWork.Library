namespace PracticalWork.Library.Contracts.v1.Enums;

/// <summary>
/// Категория книги.
/// Определяет тип литературы, к которому относится издание.
/// </summary>
public enum BookCategory
{
    /// <summary>
    /// Значение по умолчанию.
    /// Используется, если категория не указана.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Научная литература.
    /// </summary>
    ScientificBook = 10,

    /// <summary>
    /// Учебное пособие.
    /// </summary>
    EducationalBook = 20,

    /// <summary>
    /// Художественная литература.
    /// </summary>
    FictionBook = 30
}
