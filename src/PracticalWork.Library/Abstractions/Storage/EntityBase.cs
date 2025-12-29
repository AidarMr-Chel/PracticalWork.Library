namespace PracticalWork.Library.Abstractions.Storage;

/// <summary>
/// Базовый класс для всех сущностей доменной модели.
/// Содержит общие поля идентификатора и временных меток.
/// </summary>
public abstract class EntityBase : IEntity
{
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Дата и время создания сущности (UTC).
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления сущности (UTC).
    /// Может быть <c>null</c>, если обновлений не было.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Инициализирует новую сущность,
    /// автоматически назначая идентификатор и дату создания.
    /// </summary>
    protected EntityBase()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }
}
