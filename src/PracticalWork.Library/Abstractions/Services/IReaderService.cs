using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Services
{
    /// <summary>
    /// Сервис для управления читателями библиотеки.
    /// Предоставляет операции создания, продления, закрытия
    /// и получения информации о читателях и их книгах.
    /// </summary>
    public interface IReaderService
    {
        /// <summary>
        /// Создаёт нового читателя.
        /// </summary>
        /// <param name="reader">Модель читателя.</param>
        /// <returns>Идентификатор созданного читателя.</returns>
        Task<Guid> CreateReader(Reader reader);

        /// <summary>
        /// Продлевает срок действия читательского билета.
        /// </summary>
        /// <param name="id">Идентификатор читателя.</param>
        /// <param name="newDate">Новая дата окончания действия.</param>
        Task ExtendReader(Guid id, DateOnly newDate);

        /// <summary>
        /// Закрывает читателя (например, при окончании обслуживания).
        /// </summary>
        /// <param name="id">Идентификатор читателя.</param>
        Task CloseReader(Guid id);

        /// <summary>
        /// Возвращает список книг, выданных читателю.
        /// </summary>
        /// <param name="id">Идентификатор читателя.</param>
        /// <returns>Коллекция книг.</returns>
        Task<IEnumerable<Book>> GetBook(Guid id);

        /// <summary>
        /// Ищет идентификатор читателя по номеру телефона.
        /// </summary>
        /// <param name="phone">Номер телефона.</param>
        /// <returns>Идентификатор читателя или <c>null</c>, если не найден.</returns>
        Task<Guid?> FindReaderIdByPhoneAsync(string phone);

        /// <summary>
        /// Ищет идентификатор читателя по полному имени.
        /// </summary>
        /// <param name="fullName">Полное имя читателя.</param>
        /// <returns>Идентификатор читателя или <c>null</c>, если не найден.</returns>
        Task<Guid?> FindReaderIdByNameAsync(string fullName);
    }
}
