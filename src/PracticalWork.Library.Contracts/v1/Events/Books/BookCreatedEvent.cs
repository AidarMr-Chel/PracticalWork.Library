using PracticalWork.Library.Contracts.v1.Events;

namespace PracticalWork.Library.Contracts.v1.Events.Books
{
    /// <summary>
    /// Событие, возникающее при создании книги.
    /// Содержит идентификатор книги, её название,
    /// категорию и дату создания.
    /// </summary>
    public sealed class BookCreatedEvent : BaseLibraryEvent
    {
        /// <summary>
        /// Идентификатор созданной книги.
        /// </summary>
        public Guid BookId { get; init; }

        /// <summary>
        /// Название книги.
        /// </summary>
        public string Title { get; init; } = default!;

        /// <summary>
        /// Категория книги в строковом представлении.
        /// </summary>
        public string Category { get; init; } = default!;

        /// <summary>
        /// Дата и время создания книги.
        /// </summary>
        public DateTime CreatedAt { get; init; }

        /// <summary>
        /// Создаёт новое событие создания книги.
        /// </summary>
        public BookCreatedEvent()
            : base("book.created")
        {
        }
    }
}
