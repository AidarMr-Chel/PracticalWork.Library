namespace PracticalWork.Library.Models;

/// <summary>
/// Модель читателя библиотеки.
/// Используется в доменной логике и не зависит от EF Core.
/// </summary>
public sealed class Reader
{
    /// <summary>
    /// Уникальный идентификатор читателя.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Полное имя читателя.
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Контактный номер телефона.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// Дата окончания действия читательского билета.
    /// </summary>
    public DateOnly ExpiryDate { get; set; }

    /// <summary>
    /// Признак активного статуса читателя.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Email читателя. Может отсутствовать.
    /// </summary>
    public string Email { get; set; }
}
