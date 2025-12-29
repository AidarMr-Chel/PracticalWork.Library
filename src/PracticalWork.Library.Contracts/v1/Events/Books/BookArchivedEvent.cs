using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Books
{
    /// <summary>
    /// Событие, возникающее при переводе книги в архив.
    /// Содержит идентификатор книги и время архивирования.
    /// </summary>
    public sealed class BookArchivedEvent : BaseLibraryEvent
    {
        /// <summary>
        /// Идентификатор книги, переведённой в архив.
        /// </summary>
        public Guid BookId { get; init; }

        /// <summary>
        /// Дата и время архивирования книги.
        /// </summary>
        public DateTime ArchivedAt { get; init; }

        /// <summary>
        /// Создаёт новое событие архивирования книги.
        /// </summary>
        public BookArchivedEvent()
            : base("book.archived")
        {
        }
    }
}
