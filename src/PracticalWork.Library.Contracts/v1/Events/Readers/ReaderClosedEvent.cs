using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Readers
{
    public sealed class ReaderClosedEvent : BaseLibraryEvent
    {
        public Guid ReaderId { get; init; }
        public DateTime ClosedAt { get; init; }

        public ReaderClosedEvent()
            : base("reader.closed")
        {
        }
    }
}
