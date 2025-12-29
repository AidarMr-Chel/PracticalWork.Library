namespace PracticalWork.Library.Enums;

/// <summary>
/// Статус книги в библиотеке.
/// Определяет текущее состояние экземпляра.
/// </summary>
public enum BookStatus
{
    /// <summary>
    /// Книга доступна для выдачи.
    /// </summary>
    /// <remarks>Значение по умолчанию.</remarks>
    Available = 0,

    /// <summary>
    /// Книга находится у читателя (выдана).
    /// </summary>
    Borrow = 10,

    /// <summary>
    /// Книга переведена в архив и недоступна для выдачи.
    /// </summary>
    Archived = 20
}
