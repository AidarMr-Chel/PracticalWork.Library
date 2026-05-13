using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticalWork.Library.Data.PostgreSql.Entities;

[Table("activity_logs")]
public class ActivityLog
{
    [Key]
    [Column("id")]  
    public Guid Id { get; set; }

    [Column("event_type")] 
    public string EventType { get; set; } = string.Empty;

    [Column("payload")]  
    public string Payload { get; set; } = string.Empty;

    [Column("created_at")] 
    public DateTime CreatedAt { get; set; }
}