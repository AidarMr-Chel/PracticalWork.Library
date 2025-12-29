using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.v1.Events.Readers;
using PracticalWork.Library.Enums;
using PracticalWork.Library.MessageBroker.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services
{
    /// <summary>
    /// Сервис для управления читателями.
    /// Отвечает за создание, продление, закрытие карточек,
    /// а также получение информации о читателях и их книгах.
    /// </summary>
    public sealed class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;
        private readonly IMessagePublisher _publisher;

        public ReaderService(IReaderRepository readerRepository, IMessagePublisher publisher)
        {
            _readerRepository = readerRepository;
            _publisher = publisher;
        }

        /// <summary>
        /// Создаёт новую карточку читателя.
        /// Выполняет проверку уникальности номера телефона
        /// и публикует событие о создании читателя.
        /// </summary>
        /// <param name="reader">Модель читателя.</param>
        /// <returns>Идентификатор созданного читателя.</returns>
        /// <exception cref="InvalidOperationException">Если номер телефона уже используется.</exception>
        public async Task<Guid> CreateReader(Reader reader)
        {
            var existing = await _readerRepository.GetByPhoneAsync(reader.PhoneNumber);
            if (existing != null)
                throw new InvalidOperationException($"Читатель с номером {reader.PhoneNumber} уже существует.");

            reader.Id = Guid.NewGuid();
            reader.ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));
            reader.IsActive = true;

            await _readerRepository.AddAsync(reader);

            await _publisher.PublishAsync(new ReaderCreatedEvent
            {
                ReaderId = reader.Id,
                FullName = reader.FullName,
                PhoneNumber = reader.PhoneNumber,
                CreatedAt = DateTime.UtcNow
            });

            return reader.Id;
        }

        /// <summary>
        /// Продлевает срок действия карточки читателя.
        /// </summary>
        /// <param name="id">Идентификатор читателя.</param>
        /// <param name="newDate">Новая дата окончания действия.</param>
        /// <exception cref="InvalidOperationException">
        /// Если карточка не найдена, неактивна или новая дата меньше текущей.
        /// </exception>
        public async Task ExtendReader(Guid id, DateOnly newDate)
        {
            var reader = await _readerRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Карточка с id={id} не найдена");

            if (!reader.IsActive)
                throw new InvalidOperationException("Карточка неактивна");

            if (reader.ExpiryDate > newDate)
                throw new InvalidOperationException("Новая дата должна быть позже текущей");

            reader.ExpiryDate = newDate;
            await _readerRepository.UpdateAsync(reader);
        }

        /// <summary>
        /// Закрывает карточку читателя.
        /// Проверяет отсутствие невозвращённых книг и публикует событие закрытия.
        /// </summary>
        /// <param name="id">Идентификатор читателя.</param>
        /// <exception cref="InvalidOperationException">
        /// Если карточка не найдена, уже неактивна или есть невозвращённые книги.
        /// </exception>
        public async Task CloseReader(Guid id)
        {
            var reader = await _readerRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Карточка с id={id} не найдена");

            if (!reader.IsActive)
                throw new InvalidOperationException("Карточка уже неактивна");

            var books = await _readerRepository.GetBooksByReaderIdAsync(id);
            var notReturned = books.Where(b => b.Status != BookStatus.Archived).ToList();

            if (notReturned.Any())
                throw new InvalidOperationException(
                    $"У читателя есть несданные книги: {string.Join(", ", notReturned.Select(b => b.Id))}");

            reader.IsActive = false;

            await _readerRepository.UpdateAsync(reader);

            await _publisher.PublishAsync(new ReaderClosedEvent
            {
                ReaderId = id,
                ClosedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Возвращает список книг, выданных читателю.
        /// </summary>
        /// <param name="id">Идентификатор читателя.</param>
        /// <returns>Коллекция книг.</returns>
        /// <exception cref="InvalidOperationException">Если карточка не найдена или неактивна.</exception>
        public async Task<IEnumerable<Book>> GetBook(Guid id)
        {
            var reader = await _readerRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Карточка с id={id} не найдена");

            if (!reader.IsActive)
                throw new InvalidOperationException("Карточка неактивна");

            return await _readerRepository.GetBooksByReaderIdAsync(id);
        }

        /// <summary>
        /// Ищет идентификатор читателя по номеру телефона.
        /// </summary>
        /// <param name="phone">Номер телефона.</param>
        /// <returns>Идентификатор читателя или null.</returns>
        public async Task<Guid?> FindReaderIdByPhoneAsync(string phone)
        {
            var reader = await _readerRepository.GetByPhoneAsync(phone);
            return reader?.Id;
        }

        /// <summary>
        /// Ищет идентификатор читателя по полному имени.
        /// </summary>
        /// <param name="fullName">Полное имя читателя.</param>
        /// <returns>Идентификатор читателя или null.</returns>
        public async Task<Guid?> FindReaderIdByNameAsync(string fullName)
        {
            var reader = await _readerRepository.GetByNameAsync(fullName);
            return reader?.Id;
        }
    }
}
