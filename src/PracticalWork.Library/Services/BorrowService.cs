using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.v1.Events.Borrows;
using PracticalWork.Library.Enums;
using PracticalWork.Library.MessageBroker.Abstractions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services
{
    public sealed class BorrowService : IBorrowService
    {
        private readonly IBorrowRepository _borrowRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IReaderRepository _readerRepository;
        private readonly ICacheService _cache;
        private readonly IMessagePublisher _publisher;

        private const string BorrowKeysRegistry = "Borrow:Keys";
        private const string AvailableBooksKeysRegistry = "AvailableBooks:Keys";

        public BorrowService(
            IBorrowRepository borrowRepository,
            IBookRepository bookRepository,
            IReaderRepository readerRepository,
            ICacheService cache,
            IMessagePublisher publisher)
        {
            _borrowRepository = borrowRepository;
            _bookRepository = bookRepository;
            _readerRepository = readerRepository;
            _cache = cache;
            _publisher = publisher;
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

            await _cache.ClearByRegistryAsync(BorrowKeysRegistry);
            await _cache.ClearByRegistryAsync(AvailableBooksKeysRegistry);
            await _publisher.PublishAsync(new BookBorrowedEvent
            {
                BorrowId = borrowId,
                BookId = bookId,
                ReaderId = readerId,
                BorrowedAt = DateTime.UtcNow,
                DueDate = borrow.DueDate.ToDateTime(TimeOnly.MinValue)
            });

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

            await _cache.ClearByRegistryAsync(BorrowKeysRegistry);
            await _cache.ClearByRegistryAsync(AvailableBooksKeysRegistry);
            await _publisher.PublishAsync(new BookReturnedEvent
            {
                BorrowId = borrow.Id,
                BookId = bookId,
                ReaderId = borrow.ReaderId,
                ReturnedAt = DateTime.UtcNow
            });
        }

        public async Task<IEnumerable<Book>> GetAvailableBooksAsync(Book filter)
        {
            var cacheKey =
                $"AvailableBooks:{filter.Category}:{filter.Status}:{string.Join(",", filter.Authors ?? new List<string>())}:{filter.Year}";

            var cached = await _cache.GetAsync<IEnumerable<Book>>(cacheKey);
            if (cached != null)
                return cached;

            var books = await _bookRepository.FindAsync(filter);
            var available = books.Where(b => !b.IsArchived).ToList();

            await _cache.SetAsync(cacheKey, available, TimeSpan.FromMinutes(10));
            await _cache.TrackKeyAsync(AvailableBooksKeysRegistry, cacheKey);

            return available;
        }

        public async Task<Borrow> GetByIdAsync(Guid id)
        {
            var cacheKey = $"Borrow:{id}";

            var cached = await _cache.GetAsync<Borrow>(cacheKey);
            if (cached != null)
                return cached;

            var borrow = await _borrowRepository.GetByIdAsync(id);
            if (borrow != null)
            {
                await _cache.SetAsync(cacheKey, borrow, TimeSpan.FromMinutes(10));
                await _cache.TrackKeyAsync(BorrowKeysRegistry, cacheKey);
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
            if (reader == null)
                return null;

            return await _borrowRepository.GetByReaderIdAsync(reader.Id);
        }
    }
}
