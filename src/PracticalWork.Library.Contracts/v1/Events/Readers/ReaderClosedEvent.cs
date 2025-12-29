using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Readers
{
    /// <summary>
    /// Событие, возникающее при закрытии читательского профиля.
    /// Содержит идентификатор читателя и дату закрытия.
    /// </summary>
    public sealed class ReaderClosedEvent : BaseLibraryEvent
    {
        /// <summary>
        /// Идентификатор читателя, чей профиль был закрыт.
        /// </summary>
        public Guid ReaderId { get; init; }

        /// <summary>
        /// Дата и время закрытия читательского профиля.
        /// </summary>
        public DateTime ClosedAt { get; init; }

        /// <summary>
        /// Создаёт новое событие закрытия читателя.
        /// </summary>
        public ReaderClosedEvent()
            : base("reader.closed")
        {
        }
    }
}
