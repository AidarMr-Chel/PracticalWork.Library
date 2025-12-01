using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PracticalWork.Library.Enums;


namespace PracticalWork.Library.Models
{
    /// <summary>
    /// Выдача книги
    /// </summary>
    public sealed class Borrow
    {
        /// <summary>Идентификатор книги</summary>
        public Guid BookId { get; set; }

        /// <summary>Идентификатор карточки читателя</summary>
        public Guid ReaderId { get; set; }

        /// <summary>Дата выдачи книги</summary>
        public DateOnly BorrowDate { get; set; }

        /// <summary>Срок возврата книги</summary>
        public DateOnly DueDate { get; set; }

        /// <summary>Фактическая дата возврата книги</summary>
        public DateOnly ReturnDate { get; set; }

        /// <summary>Статус книги в библиотеке</summary>
        public BookIssueStatus Status { get; set; }
    }
}
