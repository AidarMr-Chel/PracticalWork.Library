using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

public sealed class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMinioService _minioService;
    private readonly IDistributedCache _cache;

    private const string CacheKeyRegistry = "Books:Keys";
    private const string DetailsCacheRegistry = "Books:Details:Keys";

    public BookService(IBookRepository bookRepository, IMinioService minioService, IDistributedCache cache)
    {
        _bookRepository = bookRepository;
        _minioService = minioService;
        _cache = cache;
    }


    public async Task<Guid> CreateBook(Book book)
    {
        book.Status = BookStatus.Available;
        var id = await _bookRepository.AddAsync(book);
        await ClearBooksCache();
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
        await ClearBooksCache();
        await ClearBookDetailsCache(id);
    }

    public async Task<Book> ArchivingBook(Guid id)
    {
        var book = await _bookRepository.GetByIdAsync(id)
            ?? throw new BookServiceException($"Книга с id={id} не найдена");

        book.Archive();
        await _bookRepository.UpdateAsync(book);
        await ClearBooksCache();
        await ClearBookDetailsCache(id);
        return book;
    }


    public async Task<IEnumerable<Book>> GetBooks(Book filter, int pageNumber = 1, int pageSize = 10)
    {
        var cacheKey = $"Books:{filter.Category}:{filter.Status}:{string.Join(",", filter.Authors ?? new List<string>())}:{filter.Year}:Page{pageNumber}:Size{pageSize}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<IEnumerable<Book>>(cached);

        var books = await _bookRepository.FindAsync(filter);

        var paged = books
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var json = JsonSerializer.Serialize(paged);
        await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
        await TrackCacheKeyAsync(CacheKeyRegistry, cacheKey);

        return paged;
    }


    public async Task<Book> GetBookDetailsAsync(Guid bookId)
    {
        var cacheKey = $"Book:{bookId}:Details";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached != null)
            return JsonSerializer.Deserialize<Book>(cached);

        var book = await _bookRepository.GetByIdAsync(bookId)
            ?? throw new BookServiceException($"Книга с id={bookId} не найдена");

        var json = JsonSerializer.Serialize(book);
        await _cache.SetStringAsync(cacheKey, json, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
        await TrackCacheKeyAsync(DetailsCacheRegistry, cacheKey);

        return book;
    }

    public async Task UpdateBookDetailsAsync(Guid bookId, string description, Stream coverStream, string fileName, string contentType)
    {
        var book = await _bookRepository.GetByIdAsync(bookId)
            ?? throw new InvalidOperationException("Книга не найдена");

        book.Description = description;

        if (coverStream != null && fileName != null && contentType != null)
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
        await ClearBooksCache();
        await ClearBookDetailsCache(bookId);
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

    private async Task ClearBooksCache()
    {
        await ClearCacheByRegistry(CacheKeyRegistry);
    }

    private async Task ClearBookDetailsCache(Guid bookId)
    {
        var cacheKey = $"Book:{bookId}:Details";
        await _cache.RemoveAsync(cacheKey);

        var existing = await _cache.GetStringAsync(DetailsCacheRegistry);
        if (existing != null)
        {
            var keys = JsonSerializer.Deserialize<HashSet<string>>(existing);
            keys.Remove(cacheKey);
            var json = JsonSerializer.Serialize(keys);
            await _cache.SetStringAsync(DetailsCacheRegistry, json);
        }
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
