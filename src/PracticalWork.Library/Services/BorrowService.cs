using Microsoft.Extensions.Caching.Distributed;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;
using System.Text.Json;

namespace PracticalWork.Library.Services
{
    public sealed class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IReaderRepository _readerRepository;
        private readonly IDistributedCache _cache;

        private const string BorrowKeysRegistry = "Borrow:Keys";
        private const string AvailableBooksKeysRegistry = "AvailableBooks:Keys";

        public BorrowService(
            IBorrowRepository borrowRepository,
            IBookRepository bookRepository,
            IReaderRepository readerRepository,
            IDistributedCache cache)
        {
            _borrowRepository = borrowRepository;
            _bookRepository = bookRepository;
            _readerRepository = readerRepository;
            _cache = cache;
        }

        public async Task<Guid> CreateBorrow(Guid bookId, Guid readerId)
        {
            var book = await _bookRepository.GetByIdAsync(bookId)
                ?? throw new InvalidOperationException($"Книга с id={bookId} не найдена");

            if (book.Status == BookStatus.Archived)
                throw new InvalidOperationException("Книга в архиве");

            if (book.Status == BookStatus.Borrow)
                throw new InvalidOperationException("Книга уже выдана");

            var reader = await _readerRepository.GetByIdAsync(readerId)
                ?? throw new InvalidOperationException($"Читатель с id={readerId} не найден");

            if (!reader.IsActive)
                throw new InvalidOperationException("Карточка неактивна");

            var borrow = new Borrow
            {
                Id = Guid.NewGuid(),
                BookId = bookId,
                ReaderId = readerId,
                BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow),
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                Status = BookIssueStatus.Issued
            };

            var borrowId = await _borrowRepository.AddBorrowAsync(borrow);

            book.Status = BookStatus.Borrow;
            await _bookRepository.UpdateAsync(book);

            await ClearCacheByRegistry(BorrowKeysRegistry);
            await ClearCacheByRegistry(AvailableBooksKeysRegistry);

            return borrowId;
        }

        public async Task ReturnBook(Guid bookId)
        {
            var borrow = await _borrowRepository.GetActiveBorrowAsync(bookId)
                ?? throw new InvalidOperationException("Нет активной выдачи для этой книги");

            borrow.ReturnDate = DateOnly.FromDateTime(DateTime.UtcNow);
            borrow.Status = BookIssueStatus.Returned;

            await _borrowRepository.UpdateBorrowAsync(borrow);

            var book = await _bookRepository.GetByIdAsync(bookId)
                ?? throw new InvalidOperationException("Книга не найдена");

            book.Status = BookStatus.Available;
            await _bookRepository.UpdateAsync(book);

            await ClearCacheByRegistry(BorrowKeysRegistry);
            await ClearCacheByRegistry(AvailableBooksKeysRegistry);
        }

        public async Task<IEnumerable<Book>> GetAvailableBooksAsync(Book filter)
        {
            var cacheKey = $"AvailableBooks:{filter.Category}:{filter.Status}:{string.Join(",", filter.Authors ?? new List<string>())}:{filter.Year}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<IEnumerable<Book>>(cached);

            var books = await _bookRepository.FindAsync(filter);
            var available = books.Where(b => !b.IsArchived).ToList();

            var json = JsonSerializer.Serialize(available);
            await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            await TrackCacheKeyAsync(AvailableBooksKeysRegistry, cacheKey);
            return available;
        }

        public async Task<Borrow> GetByIdAsync(Guid id)
        {
            var cacheKey = $"Borrow:{id}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
                return JsonSerializer.Deserialize<Borrow>(cached);

            var borrow = await _borrowRepository.GetByIdAsync(id);
            if (borrow != null)
            {
                var json = JsonSerializer.Serialize(borrow);
                await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                await TrackCacheKeyAsync(BorrowKeysRegistry, cacheKey);
            }

            return borrow;
        }

        public async Task<Borrow> GetByReaderIdAsync(Guid readerId)
        {
            return await _borrowRepository.GetByReaderIdAsync(readerId);
        }

        public async Task<Borrow> GetDetailsAsync(string idOrReader)
        {
            if (Guid.TryParse(idOrReader, out var id))
                return await GetByIdAsync(id);

            var reader = await _readerRepository.GetByNameAsync(idOrReader);
            if (reader == null) return null;

            return await _borrowRepository.GetByReaderIdAsync(reader.Id);
        }


        private async Task TrackCacheKeyAsync(string registryKey, string key)
        {
            var existing = await _cache.GetStringAsync(registryKey);
            var keys = existing != null
                ? JsonSerializer.Deserialize<HashSet<string>>(existing)
                : new HashSet<string>();

            keys.Add(key);

            var json = JsonSerializer.Serialize(keys);
            await _cache.SetStringAsync(registryKey, json);
        }

        private async Task ClearCacheByRegistry(string registryKey)
        {
            var existing = await _cache.GetStringAsync(registryKey);
            if (existing == null) return;

            var keys = JsonSerializer.Deserialize<HashSet<string>>(existing);
            foreach (var key in keys)
            {
                await _cache.RemoveAsync(key);
            }

            await _cache.RemoveAsync(registryKey);
        }
    }
}
