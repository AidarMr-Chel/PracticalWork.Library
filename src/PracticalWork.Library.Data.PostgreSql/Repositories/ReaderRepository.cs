using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Data.PostgreSql.Repositories
{
    public sealed class ReaderRepository : IReaderRepository
    {
        private readonly AppDbContext _appDbContext;

        public ReaderRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        /// <summary>
        /// Получает читателя по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Reader> GetByIdAsync(Guid id)
        {
            var entity = await _appDbContext.Readers
                .Include(r => r.BorrowedRecords)
                .FirstOrDefaultAsync(r => r.Id == id);

            return entity == null ? null : MapToModel(entity);
        }

        /// <summary>
        /// Получает читателя по номеру телефона
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public async Task<Reader> GetByPhoneAsync(string phone)
        {
            var entity = await _appDbContext.Readers
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.PhoneNumber == phone);

            return entity == null ? null : MapToModel(entity);
        }

        /// <summary>
        /// Добавляет читателя в хранилище
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task AddAsync(Reader reader)
        {
            var entity = MapToEntity(reader);
            _appDbContext.Readers.Add(entity);
            await _appDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Обновляет информацию о читателе
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public async Task UpdateAsync(Reader reader)
        {
            await _appDbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Получает книги, взятые читателем по его идентификатору
        /// </summary>
        /// <param name="readerId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Book>> GetBooksByReaderIdAsync(Guid readerId)
        {
            var bookIds = await _appDbContext.BookBorrows
                .Where(b => b.ReaderId == readerId && b.Status == BookIssueStatus.Issued)
                .Select(b => b.BookId)
                .ToListAsync();

            var books = await _appDbContext.Books
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            return books.Select(MapBookToModel);
        }

        /// <summary>
        /// Получает читателя по полному имени
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public async Task<Reader> GetByNameAsync(string fullName)
        {
            var entity = await _appDbContext.Readers
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.FullName == fullName);

            return entity == null ? null : MapToModel(entity);
        }

        /// <summary>
        /// Преобразует сущность в модель
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static Reader MapToModel(ReaderEntity entity) =>
            new Reader
            {
                Id = entity.Id,
                FullName = entity.FullName,
                PhoneNumber = entity.PhoneNumber,
                ExpiryDate = entity.ExpiryDate,
                IsActive = entity.IsActive
            };

        /// <summary>
        /// Преобразует модель в сущность
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static ReaderEntity MapToEntity(Reader model) =>
            new ReaderEntity
            {
                Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                ExpiryDate = model.ExpiryDate,
                IsActive = model.IsActive
            };

        /// <summary>
        /// Преобразует сущность книги в модель
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static Book MapBookToModel(AbstractBookEntity entity) =>
           new Book
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
                   _ => BookCategory.Default
               },
               Status = entity.Status,
               CoverImagePath = entity.CoverImagePath,
               IsArchived = entity.Status == BookStatus.Archived
           };
    }
}
