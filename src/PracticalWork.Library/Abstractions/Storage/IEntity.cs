using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticalWork.Library.Abstractions.Storage;

/// <summary>
/// Базовый интерфейс для всех сущностей хранилища.
/// Определяет обязательные поля идентификатора и временных меток.
/// </summary>
public interface IEntity
{
    /// <summary>
    /// Уникальный идентификатор сущности.
    /// Генерируется автоматически при создании записи.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    Guid Id { get; set; }

    /// <summary>
    /// Дата и время создания сущности (UTC).
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата и время последнего обновления сущности (UTC).
    /// Может быть <c>null</c>, если обновлений не было.
    /// </summary>
    DateTime? UpdatedAt { get; set; }
}
