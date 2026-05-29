using PracticalWork.Library.Models;

namespace PracticalWork.Library.Abstractions.Storage
{
    /// <summary>
    /// Репозиторий для управления сущностями книг.
    /// Определяет операции добавления, получения, обновления и поиска книг.
    /// </summary>
    public interface IBookRepository
    {
        /// <summary>
        /// Добавляет новую книгу в хранилище.
        /// </summary>
        /// <param name="book">Модель книги для добавления.</param>
        /// <returns>Идентификатор созданной книги.</returns>
        Task<Guid> AddAsync(Book book);

        /// <summary>
        /// Возвращает книгу по её идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор книги.</param>
        /// <returns>Модель книги.</returns>
        Task<Book> GetByIdAsync(Guid id);

        /// <summary>
        /// Обновляет данные существующей книги.
        /// </summary>
        /// <param name="book">Модель книги с обновлёнными данными.</param>
        Task UpdateAsync(Book book);

        /// <summary>
        /// Выполняет поиск книг по указанному фильтру.
        /// </summary>
        /// <param name="filter">Фильтр по свойствам книги.</param>
        /// <param name="excludeArchived">Исключить архивированные книги.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Коллекция найденных книг.</returns>
        Task<IReadOnlyList<Book>> FindAsync(
            Book filter,
            bool excludeArchived = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Выполняет поиск книг с пагинацией на уровне БД.
        /// </summary>
        Task<IReadOnlyList<Book>> FindPagedAsync(
            Book filter,
            int pageNumber,
            int pageSize,
            bool excludeArchived = false,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Book>> GetArchivableBooksAsync(int yearsWithoutBorrow, int limit, CancellationToken ct = default);
    }
}
