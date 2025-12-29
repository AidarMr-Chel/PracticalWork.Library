using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services
{
    public interface IBorrowService
    {
        /// <summary>
        /// Создание выдачи книги
        /// </summary>
        /// <param name="bookId"></param>
        /// <param name="readerId"></param>
        /// <returns></returns>
        Task<Guid> CreateBorrow(Guid bookId, Guid readerId);
        /// <summary>
        /// Возврат книги
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        Task ReturnBook(Guid bookId);
        /// <summary>
        /// Получение списка доступных книг по фильтру
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<Book>> GetAvailableBooksAsync(Book filter);
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
        /// <summary>
        /// Получение деталей выдачи по идентификатору или читателю
        /// </summary>
        /// <param name="idOrReader"></param>
        /// <returns></returns>
        Task<Borrow> GetDetailsAsync(string idOrReader);
    }
}
