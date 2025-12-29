using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Borrows
{
    public sealed class BookReturnedEvent : BaseLibraryEvent
    {
        public Guid BorrowId { get; init; }
        public Guid BookId { get; init; }
        public Guid ReaderId { get; init; }
        public DateTime ReturnedAt { get; init; }

        public BookReturnedEvent()
            : base("book.returned")
        {
        }
    }
}
