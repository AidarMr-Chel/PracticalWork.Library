using PracticalWork.Library.Abstractions.Storage;
using System.ComponentModel.DataAnnotations.Schema;  


namespace PracticalWork.Library.Data.PostgreSql.Entities;

/// <summary>
/// Карточка читателя
/// </summary>
[Table("readers")]
public sealed class ReaderEntity : EntityBase
{
    /// <summary>ФИО</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Номер телефона</summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>Дата окончания действия карточки</summary>
    public DateOnly ExpiryDate { get; set; }

    /// <summary>Активность карточки</summary>
    public bool IsActive { get; set; }

    /// <summary>Электронная почта читателя для уведомлений</summary>
    public string Email { get; set; }

    /// <summary>Записи о взятых книгах</summary>
    public ICollection<BookBorrowEntity> BorrowedRecords { get; set; } = new List<BookBorrowEntity>();
}