using PracticalWork.Library.Enums;

namespace PracticalWork.Library.Models
{
    /// <summary>
    /// Модель выдачи книги читателю.
    /// Содержит информацию о сроках, статусе и связанных сущностях.
    /// </summary>
    public sealed class Borrow
    {
        /// <summary>
        /// Уникальный идентификатор выдачи.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Идентификатор книги, которая была выдана.
        /// </summary>
        public Guid BookId { get; set; }

        /// <summary>
        /// Идентификатор читателя, которому выдана книга.
        /// </summary>
        public Guid ReaderId { get; set; }

        /// <summary>
        /// Дата выдачи книги.
        /// </summary>
        public DateOnly BorrowDate { get; set; }

        /// <summary>
        /// Дата, к которой книга должна быть возвращена.
        /// </summary>
        public DateOnly DueDate { get; set; }

        /// <summary>
        /// Фактическая дата возврата книги.
        /// Может быть пустой, если книга ещё не возвращена.
        /// </summary>
        public DateOnly ReturnDate { get; set; }

        /// <summary>
        /// Текущий статус выдачи.
        /// </summary>
        public BookIssueStatus Status { get; set; }
    }
}
