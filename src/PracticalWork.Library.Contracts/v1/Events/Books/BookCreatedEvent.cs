using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Books
{
    /// <summary>
    /// Событие создания книги.
    /// </summary>
    public sealed class BookCreatedEvent : BaseLibraryEvent
    {
        public Guid BookId { get; init; }
        public string Title { get; init; } = default!;
        public string Category { get; init; } = default!;
        public DateTime CreatedAt { get; init; }

        public BookCreatedEvent()
            : base("book.created")
        {
        }
    }
}
