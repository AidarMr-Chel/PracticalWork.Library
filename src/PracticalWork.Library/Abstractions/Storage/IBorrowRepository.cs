using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage
{
    /// <summary>
    /// Репозиторий для управления выдачами книг.
    /// Определяет операции получения, создания и обновления записей о выдачах.
    /// </summary>
    public interface IBorrowRepository
    {
        /// <summary>
        /// Возвращает активную выдачу книги, если она существует.
        /// </summary>
        /// <param name="bookId">Идентификатор книги.</param>
        /// <returns>Активная выдача или <c>null</c>, если книга не выдана.</returns>
        Task<Borrow> GetActiveBorrowAsync(Guid bookId);

        /// <summary>
        /// Добавляет новую запись о выдаче книги.
        /// </summary>
        /// <param name="borrow">Модель выдачи.</param>
        /// <returns>Идентификатор созданной выдачи.</returns>
        Task<Guid> AddBorrowAsync(Borrow borrow);

        /// <summary>
        /// Обновляет данные существующей выдачи.
        /// </summary>
        /// <param name="borrow">Модель выдачи с обновлёнными данными.</param>
        Task UpdateBorrowAsync(Borrow borrow);

        /// <summary>
        /// Возвращает выдачу по её идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор выдачи.</param>
        /// <returns>Модель выдачи.</returns>
        Task<Borrow> GetByIdAsync(Guid id);

        /// <summary>
        /// Возвращает выдачу по идентификатору читателя.
        /// </summary>
        /// <param name="readerId">Идентификатор читателя.</param>
        /// <returns>Модель выдачи.</returns>
        Task<Borrow> GetByReaderIdAsync(Guid readerId);
    }
}
