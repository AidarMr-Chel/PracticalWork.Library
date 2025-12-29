using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Books
{
    public sealed class BookArchivedEvent : BaseLibraryEvent
    {
        public Guid BookId { get; init; }
        public DateTime ArchivedAt { get; init; }

        public BookArchivedEvent()
            : base("book.archived")
        {
        }
    }
}
