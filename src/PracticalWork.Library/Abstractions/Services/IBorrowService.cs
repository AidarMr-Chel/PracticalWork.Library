using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services
{
    /// <summary>
    /// Сервис для управления выдачами книг.
    /// Определяет операции создания, возврата и получения информации о выдачах.
    /// </summary>
    public interface IBorrowService
    {
        /// <summary>
        /// Создаёт новую выдачу книги читателю.
        /// </summary>
        /// <param name="bookId">Идентификатор книги.</param>
        /// <param name="readerId">Идентификатор читателя.</param>
        /// <returns>Идентификатор созданной выдачи.</returns>
        Task<Guid> CreateBorrow(Guid bookId, Guid readerId);

        /// <summary>
        /// Регистрирует возврат книги.
        /// </summary>
        /// <param name="bookId">Идентификатор книги.</param>
        Task ReturnBook(Guid bookId);

        /// <summary>
        /// Возвращает список доступных для выдачи книг,
        /// отфильтрованных по указанным параметрам.
        /// </summary>
        /// <param name="filter">Фильтр по свойствам книги.</param>
        /// <returns>Коллекция доступных книг.</returns>
        Task<IEnumerable<Book>> GetAvailableBooksAsync(Book filter);

        /// <summary>
        /// Возвращает информацию о выдаче по её идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор выдачи.</param>
        /// <returns>Модель выдачи.</returns>
        Task<Borrow> GetByIdAsync(Guid id);

        /// <summary>
        /// Возвращает информацию о выдаче по идентификатору читателя.
        /// </summary>
        /// <param name="readerId">Идентификатор читателя.</param>
        /// <returns>Модель выдачи.</returns>
        Task<Borrow> GetByReaderIdAsync(Guid readerId);

        /// <summary>
        /// Возвращает детали выдачи по идентификатору выдачи или идентификатору читателя.
        /// </summary>
        /// <param name="idOrReader">
        /// Идентификатор выдачи или идентификатор читателя.
        /// Метод сам определяет, что именно передано.
        /// </param>
        /// <returns>Модель выдачи.</returns>
        Task<Borrow> GetDetailsAsync(string idOrReader);
    }
}
