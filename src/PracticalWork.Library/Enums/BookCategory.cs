namespace PracticalWork.Library.Enums;

/// <summary>
/// Категория книги.
/// Используется для классификации литературы по типу.
/// </summary>
public enum BookCategory
{
    /// <summary>
    /// Значение по умолчанию (категория не указана).
    /// </summary>
    Default = 0,

    /// <summary>
    /// Научная литература.
    /// </summary>
    ScientificBook = 10,

    /// <summary>
    /// Учебная литература и учебные пособия.
    /// </summary>
    EducationalBook = 20,

    /// <summary>
    /// Художественная литература.
    /// </summary>
    FictionBook = 30
}
