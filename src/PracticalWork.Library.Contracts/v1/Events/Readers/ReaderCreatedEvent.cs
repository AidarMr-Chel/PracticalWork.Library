using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Readers
{
    /// <summary>
    /// Событие, возникающее при создании читателя.
    /// Содержит идентификатор читателя, его ФИО,
    /// номер телефона и дату создания записи.
    /// </summary>
    public sealed class ReaderCreatedEvent : BaseLibraryEvent
    {
        /// <summary>
        /// Идентификатор созданного читателя.
        /// </summary>
        public Guid ReaderId { get; init; }

        /// <summary>
        /// Полное имя читателя.
        /// </summary>
        public string FullName { get; init; } = default!;

        /// <summary>
        /// Номер телефона читателя.
        /// </summary>
        public string PhoneNumber { get; init; } = default!;

        /// <summary>
        /// Дата и время создания читателя.
        /// </summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// Создаёт новое событие создания читателя.
        /// </summary>
        public ReaderCreatedEvent()
            : base("reader.created")
        {
        }
    }
}
