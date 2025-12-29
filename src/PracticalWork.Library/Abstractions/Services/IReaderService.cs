using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services
{
    public interface IReaderService
    {
        /// <summary>
        /// Создание читателя
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        Task<Guid> CreateReader(Reader reader);
        /// <summary>
        /// Продление читателя
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newDate"></param>
        /// <returns></returns>
        Task ExtendReader(Guid id, DateOnly newDate);
        /// <summary>
        /// Закрытие читателя
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task CloseReader(Guid id);
        /// <summary>
        /// Получение книг читателя по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<IEnumerable<Book>> GetBook(Guid id);
        /// <summary>
        /// Поиск идентификатора читателя по номеру телефона
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        Task<Guid?> FindReaderIdByPhoneAsync(string phone);
        /// <summary>
        /// Поиск идентификатора читателя по полному имени
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        Task<Guid?> FindReaderIdByNameAsync(string fullName);
    }
}
