namespace PracticalWork.Library.Models;

/// <summary>
/// DTO для записей лога событий.
/// Используется в отчётных сервисах и не зависит от EF Core.
/// </summary>
/// <param name="EventType">Тип события.</param>
/// <param name="Payload">Сырые данные события (JSON‑payload).</param>
/// <param name="CreatedAt">Дата и время создания записи.</param>
public sealed record ActivityLogDto(
    string EventType,
    string Payload,
    DateTime CreatedAt);
