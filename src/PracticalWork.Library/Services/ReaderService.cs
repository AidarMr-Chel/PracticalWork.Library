using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services
{
    public sealed class ReaderService : IReaderService
    {
        private readonly IReaderRepository _readerRepository;

        public ReaderService(IReaderRepository readerRepository)
        {
            _readerRepository = readerRepository;
        }

        public async Task<Guid> CreateReader(Reader reader)
        {
            var existing = await _readerRepository.GetByPhoneAsync(reader.PhoneNumber);
            if (existing != null)
                throw new InvalidOperationException($"Читатель с номером {reader.PhoneNumber} уже существует.");

            reader.Id = Guid.NewGuid();
            reader.ExpiryDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(1));
            reader.IsActive = true;

            await _readerRepository.AddAsync(reader);
            return reader.Id;
        }

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

        public async Task CloseReader(Guid id)
        {
            var reader = await _readerRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Карточка с id={id} не найдена");

            if (!reader.IsActive)
                throw new InvalidOperationException("Карточка уже неактивна");

            var books = await _readerRepository.GetBooksByIdsAsync(new[] { id });
            var notReturned = books.Where(b => b.Status != BookStatus.Archived).ToList();

            if (notReturned.Any())
                throw new InvalidOperationException($"У читателя есть несданные книги: {string.Join(", ", notReturned.Select(b => b.Id))}");

            reader.IsActive = false;
            await _readerRepository.UpdateAsync(reader);
        }

        public async Task<IEnumerable<Book>> GetBook(Guid id)
        {
            var reader = await _readerRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException($"Карточка с id={id} не найдена");

            if (!reader.IsActive)
                throw new InvalidOperationException("Карточка неактивна");

            var books = await _readerRepository.GetBooksByIdsAsync(new[] { id });
            return books;
        }

        public async Task<Guid?> FindReaderIdByPhoneAsync(string phone)
        {
            var reader = await _readerRepository.GetByPhoneAsync(phone);
            return reader?.Id;
        }

        public async Task<Guid?> FindReaderIdByNameAsync(string fullName)
        {
            var reader = await _readerRepository.GetByNameAsync(fullName);
            return reader?.Id;
        }
    }
}
