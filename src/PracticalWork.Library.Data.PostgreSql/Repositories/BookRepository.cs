using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

public sealed class BookRepository : IBookRepository
{
    private readonly AppDbContext _appDbContext;

    public BookRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<Guid> CreateBook(Book book)
    {
        AbstractBookEntity entity = book.Category switch
        {
            BookCategory.ScientificBook => new ScientificBookEntity(),
            BookCategory.EducationalBook => new EducationalBookEntity(),
            BookCategory.FictionBook => new FictionBookEntity(),
            _ => throw new ArgumentException($"Неподдерживаемая категория книги: {book.Category}", nameof(book.Category))
        };

        entity.Title = book.Title;
        entity.Description = book.Description;
        entity.Year = book.Year;
        entity.Authors = book.Authors;
        entity.Status = book.Status;

        _appDbContext.Add(entity);
        await _appDbContext.SaveChangesAsync();

        return entity.Id;
    }

    public async Task UpdateBook(Guid id, Book book)
    {
        var entity = await _appDbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (entity == null)
            throw new ArgumentException($"Книга с id={id} не найдена", nameof(id));

        // Обновляем только разрешённые поля — не меняем категорию
        entity.Title = book.Title;
        entity.Description = book.Description;
        entity.Year = book.Year;
        entity.Authors = book.Authors;
        entity.Status = book.Status;

        // Обновляем путь к обложке, если пришёл непустой
        if (!string.IsNullOrEmpty(book.CoverImagePath))
            entity.CoverImagePath = book.CoverImagePath;

        _appDbContext.Update(entity);
        await _appDbContext.SaveChangesAsync();
    }

    public async Task<Book> ArchivingBook(Guid id)
    {
        var entity = await _appDbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
        if (entity == null)
            throw new ArgumentException($"Книга с id={id} не найдена", nameof(id));

        if (entity.Status == BookStatus.Borrow)
            throw new InvalidOperationException("Невозможно заархивировать книгу, которая выдана читателю.");

        entity.Status = BookStatus.Archived;

        _appDbContext.Update(entity);
        await _appDbContext.SaveChangesAsync();
        return new Book
        {
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
            IsArchived = true
        };
    }

    public async Task<IEnumerable<Book>> GetBooks(Book book)
    {
        var query = _appDbContext.Books.AsQueryable();
        query = query.Where(b => b.Status == book.Status);
        query = query.Where(b =>
        (b is ScientificBookEntity && book.Category == BookCategory.ScientificBook) ||
        (b is EducationalBookEntity && book.Category == BookCategory.EducationalBook) ||
        (b is FictionBookEntity && book.Category == BookCategory.FictionBook));
        query = query.Where(b =>
            b.Authors.Count == book.Authors.Count &&
            b.Authors.All(a => book.Authors.Contains(a)));

        var entities = await query.ToListAsync();
        return entities.Select(entity => new Book
        {
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
        });
    }
}