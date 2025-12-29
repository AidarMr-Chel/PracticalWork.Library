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

        /// <summary>
        /// Получает активную выдачу книги по идентификатору книги
        /// </summary>
        /// <param name="bookId"></param>
        /// <returns></returns>
        public async Task<Borrow> GetActiveBorrowAsync(Guid bookId)
        {
            var entity = await _context.BookBorrows
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.Status == BookIssueStatus.Issued);

            return entity == null ? null : MapToModel(entity);
        }

        /// <summary>
        /// Добавляет запись о выдаче книги
        /// </summary>
        /// <param name="borrow"></param>
        /// <returns></returns>
        public async Task<Guid> AddBorrowAsync(Borrow borrow)
        {
            var entity = MapToEntity(borrow);
            _context.BookBorrows.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Обновляет запись о выдаче книги
        /// </summary>
        /// <param name="borrow"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task UpdateBorrowAsync(Borrow borrow)
        {
            var entity = await _context.BookBorrows.FirstOrDefaultAsync(b => b.Id == borrow.Id);
            if (entity == null) throw new InvalidOperationException("Запись не найдена");

            entity.ReturnDate = borrow.ReturnDate;
            entity.Status = borrow.Status;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Преобразует сущность в модель
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Преобразует модель в сущность
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Получает запись о выдаче книги по идентификатору
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Borrow> GetByIdAsync(Guid id)
        {
            var entity = await _context.BookBorrows.FirstOrDefaultAsync(b => b.Id == id);
            return entity == null ? null : MapToModel(entity);
        }

        /// <summary>
        /// Получает последнюю запись о выдаче книги по идентификатору читателя
        /// </summary>
        /// <param name="readerId"></param>
        /// <returns></returns>
        public async Task<Borrow> GetByReaderIdAsync(Guid readerId)
        {
            var entity = await _context.BookBorrows
                .OrderByDescending(b => b.BorrowDate)
                .FirstOrDefaultAsync(b => b.ReaderId == readerId);

            return entity == null ? null : MapToModel(entity);
        }

    }
}
