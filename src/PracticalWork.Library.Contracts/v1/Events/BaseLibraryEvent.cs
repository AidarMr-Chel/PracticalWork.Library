namespace PracticalWork.Library.Contracts.v1.Events
{
    /// <summary>
    /// Базовое событие для всех событий сервиса «Библиотека».
    /// Содержит общие метаданные, такие как идентификатор события,
    /// время возникновения, тип и источник.
    /// </summary>
    public abstract class BaseLibraryEvent
    {
        /// <summary>
        /// Уникальный идентификатор события.
        /// Генерируется автоматически при создании экземпляра.
        /// </summary>
        public Guid EventId { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Дата и время возникновения события (в формате UTC).
        /// Устанавливается автоматически при создании события.
        /// </summary>
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Тип события (например: <c>book.created</c>, <c>reader.closed</c>).
        /// Используется для маршрутизации и обработки событий.
        /// </summary>
        public string EventType { get; init; }

        /// <summary>
        /// Источник события.
        /// По умолчанию — <c>library-service</c>.
        /// </summary>
        public string Source { get; init; } = "library-service";

        /// <summary>
        /// Создаёт новое базовое событие с указанным типом.
        /// </summary>
        /// <param name="eventType">Тип события.</param>
        protected BaseLibraryEvent(string eventType)
        {
            EventType = eventType;
        }
    }
}
