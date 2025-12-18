namespace PracticalWork.Reports.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string EventType { get; set; } = default!;

        public string Payload { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
    }

}
