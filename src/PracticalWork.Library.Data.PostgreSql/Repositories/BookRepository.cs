using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для работы с книгами.
/// Содержит операции добавления, получения, обновления и поиска книг.
/// </summary>
public sealed class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;

    public BookRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Добавляет новую книгу в хранилище.
    /// </summary>
    /// <param name="book">Модель книги.</param>
    /// <returns>Идентификатор созданной книги.</returns>
    public async Task<Guid> AddAsync(Book book)
    {
        var entity = MapBookToEntity(book);
        _context.Add(entity);
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    /// <summary>
    /// Получает книгу по её идентификатору.
    /// </summary>
    /// <param name="id">Идентификатор книги.</param>
    /// <returns>Модель книги или null, если книга не найдена.</returns>
    public async Task<Book> GetByIdAsync(Guid id)
    {
        var entity = await _context.Books.FirstOrDefaultAsync(b => b.Id == id);
        return entity is null ? null : MapEntityToBook(entity);
    }

    /// <summary>
    /// Обновляет данные существующей книги.
    /// </summary>
    /// <param name="book">Модель книги с обновлёнными данными.</param>
    /// <exception cref="ArgumentException">Если книга не найдена.</exception>
    public async Task UpdateAsync(Book book)
    {
        var entity = await _context.Books.FirstOrDefaultAsync(b => b.Id == book.Id);
        if (entity is null)
            throw new ArgumentException($"Книга с id={book.Id} не найдена");

        MapBookToEntity(book, entity);
        _context.Update(entity);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Ищет книги по заданному фильтру.
    /// Поддерживает фильтрацию по статусу, категории и авторам.
    /// </summary>
    /// <param name="filter">Фильтр поиска.</param>
    /// <returns>Коллекция найденных книг.</returns>
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

    /// <summary>
    /// Создаёт сущность книги на основе модели.
    /// Определяет тип сущности по категории книги.
    /// </summary>
    /// <param name="book">Модель книги.</param>
    /// <returns>Сущность книги.</returns>
    /// <exception cref="ArgumentException">Если категория книги не поддерживается.</exception>
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

    /// <summary>
    /// Копирует данные из модели книги в сущность.
    /// Используется как при создании, так и при обновлении.
    /// </summary>
    /// <param name="book">Модель книги.</param>
    /// <param name="entity">Сущность книги.</param>
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

    /// <summary>
    /// Преобразует сущность книги в модель.
    /// Определяет категорию книги по типу сущности.
    /// </summary>
    /// <param name="entity">Сущность книги.</param>
    /// <returns>Модель книги.</returns>
    /// <exception cref="ArgumentException">Если тип сущности не поддерживается.</exception>
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
