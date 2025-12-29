using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Borrows
{
    /// <summary>
    /// Событие, возникающее при выдаче книги читателю.
    /// Содержит идентификаторы выдачи, книги, читателя,
    /// а также дату выдачи и дату, к которой книга должна быть возвращена.
    /// </summary>
    public sealed class BookBorrowedEvent : BaseLibraryEvent
    {
        /// <summary>
        /// Идентификатор операции выдачи.
        /// </summary>
        public Guid BorrowId { get; init; }

        /// <summary>
        /// Идентификатор выданной книги.
        /// </summary>
        public Guid BookId { get; init; }

        /// <summary>
        /// Идентификатор читателя, которому выдана книга.
        /// </summary>
        public Guid ReaderId { get; init; }

        /// <summary>
        /// Дата и время выдачи книги.
        /// </summary>
        public DateTime BorrowedAt { get; init; }

        /// <summary>
        /// Дата, к которой книга должна быть возвращена.
        /// </summary>
        public DateTime DueDate { get; init; }

        /// <summary>
        /// Создаёт новое событие выдачи книги.
        /// </summary>
        public BookBorrowedEvent()
            : base("book.borrowed")
        {
        }
    }
}
