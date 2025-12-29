using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Borrows
{
    public sealed class BookBorrowedEvent : BaseLibraryEvent
    {
        public Guid BorrowId { get; init; }
        public Guid BookId { get; init; }
        public Guid ReaderId { get; init; }
        public DateTime BorrowedAt { get; init; }
        public DateTime DueDate { get; init; }

        public BookBorrowedEvent()
            : base("book.borrowed")
        {
        }
    }
}
