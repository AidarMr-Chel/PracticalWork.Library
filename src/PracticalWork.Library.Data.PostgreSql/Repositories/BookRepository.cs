using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с книгами.
/// </summary>
public sealed class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Guid> AddAsync(Book book)
    {
        var entity = MapBookToEntity(book);
        _context.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    /// <inheritdoc />
    public async Task<Book> GetByIdAsync(Guid id)
    {
        var entity = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        return entity is null ? null : MapEntityToBook(entity);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(Book book)
    {
        var entity = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        if (entity is null)
            throw new ArgumentException($"Книга с id={book.Id} не найдена");

        MapBookToEntity(book, entity);
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Book>> FindAsync(
        Book filter,
        bool excludeArchived = false,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(_context.Books.AsQueryable(), filter, excludeArchived);
        var entities = await query.ToListAsync(cancellationToken);
        return entities.Select(MapEntityToBook).ToList();
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Book>> FindPagedAsync(
        Book filter,
        int pageNumber,
        int pageSize,
        bool excludeArchived = false,
        CancellationToken cancellationToken = default)
    {
        var query = ApplyFilter(_context.Books.AsQueryable(), filter, excludeArchived);

        var entities = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return entities.Select(MapEntityToBook).ToList();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Book>> GetArchivableBooksAsync(
        int yearsWithoutBorrow,
        int limit,
        CancellationToken ct = default)
    {
        var cutoffDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(-yearsWithoutBorrow));

        var lastBorrowQuery = _context.BookBorrows
            .GroupBy(b => b.BookId)
            .Select(g => new
            {
                BookId = g.Key,
                LastBorrowDate = g.Max(x => x.BorrowDate)
            });

        var entities = await _context.Books
            .Where(b => b.Status == BookStatus.Available)
            .Where(b => b.Status != BookStatus.Archived)
            .Where(b =>
                !lastBorrowQuery.Any(lb => lb.BookId == b.Id) ||
                lastBorrowQuery
                    .Where(lb => lb.BookId == b.Id)
                    .Select(lb => lb.LastBorrowDate)
                    .First() < cutoffDate
            )
            .Take(limit)
            .ToListAsync(ct);

        return entities.Select(MapEntityToBook);
    }

    private static IQueryable<AbstractBookEntity> ApplyFilter(
        IQueryable<AbstractBookEntity> query,
        Book filter,
        bool excludeArchived)
    {
        if (excludeArchived)
            query = query.Where(b => b.Status != BookStatus.Archived);

        if (filter.Status != default)
            query = query.Where(b => b.Status == filter.Status);

        if (filter.Category != default)
        {
            query = query.Where(b =>
                (b is ScientificBookEntity && filter.Category == BookCategory.ScientificBook) ||
                (b is EducationalBookEntity && filter.Category == BookCategory.EducationalBook) ||
                (b is FictionBookEntity && filter.Category == BookCategory.FictionBook));
        }

        if (filter.Authors?.Any() == true)
        {
            query = query.Where(b =>
                b.Authors.Count == filter.Authors.Count &&
                b.Authors.All(a => filter.Authors.Contains(a)));
        }

        return query;
    }

    private static AbstractBookEntity MapBookToEntity(Book book)
    {
        AbstractBookEntity entity = book.Category switch
        {
            BookCategory.ScientificBook => new ScientificBookEntity(),
            BookCategory.EducationalBook => new EducationalBookEntity(),
            BookCategory.FictionBook => new FictionBookEntity(),
            _ => throw new ArgumentException($"Неподдерживаемая категория книги: {book.Category}")
        };

        MapBookToEntity(book, entity);
        return entity;
    }

    private static void MapBookToEntity(Book book, AbstractBookEntity entity)
    {
        entity.Id = book.Id == Guid.Empty ? Guid.NewGuid() : book.Id;
        entity.Title = book.Title;
        entity.Description = book.Description;
        entity.Year = book.Year;
        entity.Authors = book.Authors.ToList();
        entity.Status = book.Status;

        if (!string.IsNullOrEmpty(book.CoverImagePath))
            entity.CoverImagePath = book.CoverImagePath;
    }

    private static Book MapEntityToBook(AbstractBookEntity entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Authors = entity.Authors,
        Description = entity.Description,
        Year = entity.Year,
        Category = entity switch
        {
            ScientificBookEntity => BookCategory.ScientificBook,
            EducationalBookEntity => BookCategory.EducationalBook,
            FictionBookEntity => BookCategory.FictionBook,
            _ => throw new ArgumentException($"Неподдерживаемая категория книги: {entity.GetType().Name}")
        },
        Status = entity.Status,
        CoverImagePath = entity.CoverImagePath,
        IsArchived = entity.Status == BookStatus.Archived
    };
}
