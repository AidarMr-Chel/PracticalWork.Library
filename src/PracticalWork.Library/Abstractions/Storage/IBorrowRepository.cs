using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage
{
    public interface IBorrowRepository
    {
        /// <summary>
        /// Получение активной выдачи по идентификатору книги
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        Task<Borrow> GetActiveBorrowAsync(Guid bookId);
        /// <summary>
        /// Добавление выдачи книги
        /// </summary>
        /// <param name="borrow"></param>
        /// <returns></returns>
        Task<Guid> AddBorrowAsync(Borrow borrow);
        /// <summary>
        /// Обновление выдачи книги
        /// </summary>
        /// <param name="borrow"></param>
        /// <returns></returns>
        Task UpdateBorrowAsync(Borrow borrow);
        /// <summary>
        /// Получение выдачи по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Borrow> GetByIdAsync(Guid id);
        /// <summary>
        /// Получение выдачи по идентификатору читателя
        /// </summary>
        /// <param name="readerId"></param>
        /// <returns></returns>
        Task<Borrow> GetByReaderIdAsync(Guid readerId);
    }
}
