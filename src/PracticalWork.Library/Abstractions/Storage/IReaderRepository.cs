using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage
{
    public interface IReaderRepository
    {
        /// <summary>
        /// Получение читателя по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Reader> GetByIdAsync(Guid id);
        /// <summary>
        /// Получение читателя по номеру телефона
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<Reader> GetByPhoneAsync(string phone);
        /// <summary>
        /// Добавление читателя
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        Task AddAsync(Reader reader);
        /// <summary>
        /// Обновление читателя
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        Task UpdateAsync(Reader reader);
        /// <summary>
        /// Получение книг читателя по идентификатору
        /// </summary>
        /// <param name="readerId"></param>
        /// <returns></returns>
        Task<IEnumerable<Book>> GetBooksByReaderIdAsync(Guid readerId);
        /// <summary>
        /// Получение читателя по полному имени
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        Task<Reader> GetByNameAsync(string fullName);
    }
}
