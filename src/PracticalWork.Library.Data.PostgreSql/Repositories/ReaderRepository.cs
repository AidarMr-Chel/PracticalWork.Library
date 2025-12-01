using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;
using Microsoft.EntityFrameworkCore;

namespace PracticalWork.Library.Data.PostgreSql.Repositories
{
    public sealed class ReaderRepository : IReaderRepository
    {
        private readonly AppDbContext _appDbContext;
        public ReaderRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Guid> CreateReader(Reader reader)
        {
            var existingReader = await _appDbContext.Readers
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.PhoneNumber == reader.PhoneNumber);

            if (existingReader != null)
            {
                throw new InvalidOperationException($"Читатель с номером {reader.PhoneNumber} уже существует.");
            }
            var readerEntity = new ReaderEntity
            {
                Id = Guid.NewGuid(),
                FullName = reader.FullName,
                PhoneNumber = reader.PhoneNumber,
                ExpiryDate = DateOnly.MinValue,
                IsActive = true
            };
            _appDbContext.Readers.Add(readerEntity);
            await _appDbContext.SaveChangesAsync();
            return readerEntity.Id;
        }
        public async Task ExtendReader(Guid id, DateOnly newDate)
        {
            var entity = await _appDbContext.Readers.FirstOrDefaultAsync(b => b.Id == id);
            if (entity == null)
                throw new InvalidOperationException($"Каточка с id={id} не найдена");
            if (entity.ExpiryDate > newDate)
                throw new InvalidOperationException($"Новая дата требуется после истечения текущего срока действия");
            if (!entity.IsActive)
                throw new InvalidOperationException($"Карточка неактивна");

            entity.ExpiryDate = newDate;

            _appDbContext.Update(entity);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task CloseReader(Guid id)
        {
            var entity = await _appDbContext.Readers
                .Include(r => r.BorrowedRecords)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (entity == null)
                throw new InvalidOperationException($"Карточка с id={id} не найдена");

            if (!entity.IsActive)
                throw new InvalidOperationException("Карточка уже неактивна");

            var notReturned = entity.BorrowedRecords
                .Where(r => r.ReturnDate == default)
                .ToList();

            if (notReturned.Any())
            {
                var bookList = string.Join(", ", notReturned.Select(r => r.BookId));
                throw new InvalidOperationException($"У читателя есть несданные книги: {bookList}");
            }

            entity.IsActive = false;
            _appDbContext.Update(entity);
            await _appDbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Book>> GetBook(Guid id)
        {
            var reader = await _appDbContext.Readers
                .Include(r => r.BorrowedRecords)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reader == null)
                throw new InvalidOperationException($"Карточка с id={id} не найдена");

            if (!reader.IsActive)
                throw new InvalidOperationException("Карточка неактивна");

            var notReturnedBookIds = reader.BorrowedRecords
                .Where(r => r.ReturnDate == default)
                .Select(r => r.BookId)
                .ToList();

            if (!notReturnedBookIds.Any())
                return new List<Book>();

            var bookEntities = await _appDbContext.Books
                .Where(b => notReturnedBookIds.Contains(b.Id))
                .ToListAsync();

            var result = bookEntities.Select(entity => new Book
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
            }).ToList();

            return result;
        }

    }
}