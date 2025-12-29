using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Readers
{
    public sealed class ReaderCreatedEvent : BaseLibraryEvent
    {
        public Guid ReaderId { get; init; }
        public string FullName { get; init; } = default!;
        public string PhoneNumber { get; init; } = default!;
        public DateTime CreatedAt { get; init; }

        public ReaderCreatedEvent()
            : base("reader.created")
        {
        }
    }
}
