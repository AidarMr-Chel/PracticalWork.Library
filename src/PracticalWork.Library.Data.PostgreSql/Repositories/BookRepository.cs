using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddAsync(Book book)
    {
        var entity = MapBookToEntity(book);
        _context.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<Book> GetByIdAsync(Guid id)
    {
        var entity = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        return entity is null ? null : MapEntityToBook(entity);
    }

    public async Task UpdateAsync(Book book)
    {
        var entity = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        if (entity is null)
            throw new ArgumentException($"Книга с id={book.Id} не найдена");

        MapBookToEntity(book, entity);
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Book>> FindAsync(Book filter)
    {
        var query = _context.Books.AsQueryable();

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

        var entities = await query.ToListAsync();
        return entities.Select(MapEntityToBook);
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
