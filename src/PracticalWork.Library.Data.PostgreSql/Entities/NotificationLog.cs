using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PracticalWork.Library.Data.PostgreSql.Entities;

[Table("notification_logs")]
public class NotificationLogEntity
{
    [Key]
    public Guid Id { get; set; }

    [Column("borrow_id")]
    public Guid BorrowId { get; set; }

    [Column("type")]
    public string Type { get; set; } = string.Empty; // "Reminder", "Report"

    [Column("sent_at")]
    public DateTime SentAt { get; set; }

    [Column("status")]
    public string Status { get; set; } = string.Empty; // "Sent", "Failed"

    [Column("error_message")]
    public string ErrorMessage { get; set; }
}