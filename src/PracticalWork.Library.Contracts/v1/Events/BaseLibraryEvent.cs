namespace PracticalWork.Library.Contracts.v1.Events
{
    /// <summary>
    /// Базовое событие для всех событий сервиса "Библиотека".
    /// </summary>
    public abstract class BaseLibraryEvent
    {
        /// <summary>
        /// Уникальный идентификатор события.
        /// </summary>
        public Guid EventId { get; init; } = Guid.NewGuid();

        /// <summary>
        /// Дата и время возникновения события (UTC).
        /// </summary>
        public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

        /// <summary>
        /// Тип события (например: book.created, reader.closed).
        /// </summary>
        public string EventType { get; init; }

        /// <summary>
        /// Источник события.
        /// </summary>
        public string Source { get; init; } = "library-service";

        protected BaseLibraryEvent(string eventType)
        {
            EventType = eventType;
        }
    }
}
