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

        public async Task<Reader> GetByIdAsync(Guid id)
        {
            var entity = await _appDbContext.Readers
                .Include(r => r.BorrowedRecords)
                .FirstOrDefaultAsync(r => r.Id == id);

            return entity == null ? null : MapToModel(entity);
        }

        public async Task<Reader> GetByPhoneAsync(string phone)
        {
            var entity = await _appDbContext.Readers
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.PhoneNumber == phone);

            return entity == null ? null : MapToModel(entity);
        }

        public async Task AddAsync(Reader reader)
        {
            var entity = MapToEntity(reader);
            _appDbContext.Readers.Add(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Reader reader)
        {
            var entity = MapToEntity(reader);
            _appDbContext.Update(entity);
            await _appDbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<Guid> ids)
        {
            var entities = await _appDbContext.Books
                .Where(b => ids.Contains(b.Id))
                .ToListAsync();

            return entities.Select(MapBookToModel);
        }
        public async Task<Reader> GetByNameAsync(string fullName)
        {
            var entity = await _appDbContext.Readers
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.FullName == fullName);

            return entity == null ? null : MapToModel(entity);
        }


        private static Reader MapToModel(ReaderEntity entity) =>
            new Reader
            {
                Id = entity.Id,
                FullName = entity.FullName,
                PhoneNumber = entity.PhoneNumber,
                ExpiryDate = entity.ExpiryDate,
                IsActive = entity.IsActive
            };

        private static ReaderEntity MapToEntity(Reader model) =>
            new ReaderEntity
            {
                Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                ExpiryDate = model.ExpiryDate,
                IsActive = model.IsActive
            };

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
