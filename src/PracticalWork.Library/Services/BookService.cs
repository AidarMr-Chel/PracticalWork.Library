using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.MessageBroker.Abstractions;
using PracticalWork.Library.Models;
using PracticalWork.Library.Contracts.v1.Events.Books;

public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMinioService _minioService;
    private readonly ICacheService _cache;
    private readonly IMessagePublisher _publisher;

    private const string BooksRegistry = "Books:Keys";
    private const string DetailsRegistry = "Books:Details:Keys";

    public BookService(
        IBookRepository bookRepository,
        IMinioService minioService,
        ICacheService cache,
        IMessagePublisher publisher)
    {
        _bookRepository = bookRepository;
        _minioService = minioService;
        _cache = cache;
        _publisher = publisher;
    }

    public async Task<Guid> CreateBook(Book book)
    {
        book.Status = BookStatus.Available;
        var id = await _bookRepository.AddAsync(book);

        await _cache.ClearByRegistryAsync(BooksRegistry);
        await _publisher.PublishAsync(new BookCreatedEvent
        {
            BookId = id,
            Title = book.Title,
            Category = book.Category.ToString(),
            CreatedAt = DateTime.UtcNow
        });
        return id;
    }

    public async Task UpdateBook(Guid id, Book book)
    {
        var existing = await _bookRepository.GetByIdAsync(id)
            ?? throw new BookServiceException($"Книга с id={id} не найдена");

        if (existing.Category != book.Category)
            throw new BookServiceException("Нельзя изменить категорию книги");

        book.Id = id;
        await _bookRepository.UpdateAsync(book);

        await _cache.ClearByRegistryAsync(BooksRegistry);
        await _cache.RemoveAsync($"Book:{id}:Details");
    }

    public async Task<Book> ArchivingBook(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id)
            ?? throw new BookServiceException($"Книга с id={id} не найдена");

        book.Archive();
        await _bookRepository.UpdateAsync(book);

        await _cache.ClearByRegistryAsync(BooksRegistry);
        await _cache.RemoveAsync($"Book:{id}:Details");
        await _publisher.PublishAsync(new BookArchivedEvent
        {
            BookId = id,
            ArchivedAt = DateTime.UtcNow
        });

        return book;
    }

    public async Task<IEnumerable<Book>> GetBooks(Book filter, int pageNumber = 1, int pageSize = 10)
    {
        var cacheKey = $"Books:{filter.Category}:{filter.Status}:{string.Join(",", filter.Authors ?? new List<string>())}:{filter.Year}:Page{pageNumber}:Size{pageSize}";

        var cached = await _cache.GetAsync<IEnumerable<Book>>(cacheKey);
        if (cached != null)
            return cached;

        var books = await _bookRepository.FindAsync(filter);

        var paged = books
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        await _cache.SetAsync(cacheKey, paged, TimeSpan.FromMinutes(10));
        await _cache.TrackKeyAsync(BooksRegistry, cacheKey);

        return paged;
    }

    public async Task<Book> GetBookDetailsAsync(Guid bookId)
    {
        var cacheKey = $"Book:{bookId}:Details";

        var cached = await _cache.GetAsync<Book>(cacheKey);
        if (cached != null)
            return cached;

        var book = await _bookRepository.GetByIdAsync(bookId)
            ?? throw new BookServiceException($"Книга с id={bookId} не найдена");

        await _cache.SetAsync(cacheKey, book, TimeSpan.FromMinutes(10));
        await _cache.TrackKeyAsync(DetailsRegistry, cacheKey);

        return book;
    }

    public async Task UpdateBookDetailsAsync(Guid bookId, string description, Stream coverStream, string fileName, string contentType)
    {
        var book = await _bookRepository.GetByIdAsync(bookId)
            ?? throw new InvalidOperationException("Книга не найдена");

        book.Description = description;

        if (coverStream != null)
        {
            if (!contentType.StartsWith("image/"))
                throw new InvalidOperationException("Файл должен быть изображением");

            if (coverStream.Length > 5_000_000)
                throw new InvalidOperationException("Размер обложки превышает 5MB");

            var objectName = $"covers/{bookId}/{fileName}";
            var path = await _minioService.UploadAsync(coverStream, objectName, contentType);
            book.CoverImagePath = path;
        }

        await _bookRepository.UpdateAsync(book);

        await _cache.ClearByRegistryAsync(BooksRegistry);
        await _cache.RemoveAsync($"Book:{bookId}:Details");
    }
}
