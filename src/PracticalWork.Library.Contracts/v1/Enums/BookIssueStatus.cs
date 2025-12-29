namespace PracticalWork.Library.Contracts.v1.Enums;

/// <summary>
/// Статус выдачи книги.
/// Определяет текущее состояние операции выдачи.
/// </summary>
public enum BookIssueStatus
{
    /// <summary>
    /// Книга выдана читателю.
    /// </summary>
    /// <remarks>Значение по умолчанию.</remarks>
    Issued = 0,

    /// <summary>
    /// Книга возвращена в срок.
    /// </summary>
    Returned = 10,

    /// <summary>
    /// Книга возвращена с просрочкой.
    /// </summary>
    Overdue = 20
}
