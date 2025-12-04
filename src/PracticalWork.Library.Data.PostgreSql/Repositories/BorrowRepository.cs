using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Data.PostgreSql.Repositories
{
    public sealed class BorrowRepository : IBorrowRepository
    {
        private readonly AppDbContext _context;

        public BorrowRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Borrow> GetActiveBorrowAsync(Guid bookId)
        {
            var entity = await _context.BookBorrows
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.Status == BookIssueStatus.Issued);

            return entity == null ? null : MapToModel(entity);
        }

        public async Task<Guid> AddBorrowAsync(Borrow borrow)
        {
            var entity = MapToEntity(borrow);
            _context.BookBorrows.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task UpdateBorrowAsync(Borrow borrow)
        {
            var entity = await _context.BookBorrows.FirstOrDefaultAsync(b => b.Id == borrow.Id);
            if (entity == null) throw new InvalidOperationException("Запись не найдена");

            entity.ReturnDate = borrow.ReturnDate;
            entity.Status = borrow.Status;

            await _context.SaveChangesAsync();
        }


        private static Borrow MapToModel(BookBorrowEntity entity) => new()
        {
            Id = entity.Id,
            BookId = entity.BookId,
            ReaderId = entity.ReaderId,
            BorrowDate = entity.BorrowDate,
            DueDate = entity.DueDate,
            ReturnDate = entity.ReturnDate,
            Status = entity.Status
        };

        private static BookBorrowEntity MapToEntity(Borrow model) => new()
        {
            Id = model.Id == Guid.Empty ? Guid.NewGuid() : model.Id,
            BookId = model.BookId,
            ReaderId = model.ReaderId,
            BorrowDate = model.BorrowDate,
            DueDate = model.DueDate,
            ReturnDate = model.ReturnDate,
            Status = model.Status
        };
        public async Task<Borrow> GetByIdAsync(Guid id)
        {
            var entity = await _context.BookBorrows.FirstOrDefaultAsync(b => b.Id == id);
            return entity == null ? null : MapToModel(entity);
        }

        public async Task<Borrow> GetByReaderIdAsync(Guid readerId)
        {
            var entity = await _context.BookBorrows
                .OrderByDescending(b => b.BorrowDate)
                .FirstOrDefaultAsync(b => b.ReaderId == readerId);

            return entity == null ? null : MapToModel(entity);
        }

    }
}
