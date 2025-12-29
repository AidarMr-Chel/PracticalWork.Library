using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage
{
    /// <summary>
    /// Репозиторий для управления сущностями читателей.
    /// Определяет операции получения, добавления, обновления
    /// и выборки связанных данных.
    /// </summary>
    public interface IReaderRepository
    {
        /// <summary>
        /// Возвращает читателя по его идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор читателя.</param>
        /// <returns>Модель читателя.</returns>
        Task<Reader> GetByIdAsync(Guid id);

        /// <summary>
        /// Возвращает читателя по номеру телефона.
        /// </summary>
        /// <param name="phone">Номер телефона читателя.</param>
        /// <returns>Модель читателя.</returns>
        Task<Reader> GetByPhoneAsync(string phone);

        /// <summary>
        /// Добавляет нового читателя в хранилище.
        /// </summary>
        /// <param name="reader">Модель читателя.</param>
        Task AddAsync(Reader reader);

        /// <summary>
        /// Обновляет данные существующего читателя.
        /// </summary>
        /// <param name="reader">Модель читателя с обновлёнными данными.</param>
        Task UpdateAsync(Reader reader);

        /// <summary>
        /// Возвращает список книг, выданных указанному читателю.
        /// </summary>
        /// <param name="readerId">Идентификатор читателя.</param>
        /// <returns>Коллекция книг.</returns>
        Task<IEnumerable<Book>> GetBooksByReaderIdAsync(Guid readerId);

        /// <summary>
        /// Возвращает читателя по его полному имени.
        /// </summary>
        /// <param name="fullName">Полное имя читателя.</param>
        /// <returns>Модель читателя.</returns>
        Task<Reader> GetByNameAsync(string fullName);
    }
}
