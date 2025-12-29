using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Borrows
{
    /// <summary>
    /// Событие, возникающее при возврате книги.
    /// Содержит идентификаторы выдачи, книги, читателя
    /// и дату фактического возврата.
    /// </summary>
    public sealed class BookReturnedEvent : BaseLibraryEvent
    {
        /// <summary>
        /// Идентификатор операции выдачи.
        /// </summary>
        public Guid BorrowId { get; init; }

        /// <summary>
        /// Идентификатор возвращённой книги.
        /// </summary>
        public Guid BookId { get; init; }

        /// <summary>
        /// Идентификатор читателя, вернувшего книгу.
        /// </summary>
        public Guid ReaderId { get; init; }

        /// <summary>
        /// Дата и время фактического возврата книги.
        /// </summary>
        public DateTime ReturnedAt { get; init; }

        /// <summary>
        /// Создаёт новое событие возврата книги.
        /// </summary>
        public BookReturnedEvent()
            : base("book.returned")
        {
        }
    }
}
