using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Data.PostgreSql.Repositories
{
    /// <summary>
    /// Репозиторий для работы с записями о выдаче книг.
    /// Содержит операции получения, добавления и обновления выдач.
    /// </summary>
    public sealed class BorrowRepository : IBorrowRepository
    {
        private readonly AppDbContext _context;

        public BorrowRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получает активную (текущую) выдачу книги по её идентификатору.
        /// Активной считается выдача со статусом <see cref="BookIssueStatus.Issued"/>.
        /// </summary>
        /// <param name="bookId">Идентификатор книги.</param>
        /// <returns>
        /// Модель выдачи или null, если активная выдача отсутствует.
        /// </returns>
        public async Task<Borrow> GetActiveBorrowAsync(Guid bookId)
        {
            var entity = await _context.BookBorrows
                .FirstOrDefaultAsync(b => b.BookId == bookId && b.Status == BookIssueStatus.Issued);

            return entity == null ? null : MapToModel(entity);
        }

        /// <summary>
        /// Добавляет новую запись о выдаче книги.
        /// </summary>
        /// <param name="borrow">Модель выдачи.</param>
        /// <returns>Идентификатор созданной записи.</returns>
        public async Task<Guid> AddBorrowAsync(Borrow borrow)
        {
            var entity = MapToEntity(borrow);
            _context.BookBorrows.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Обновляет существующую запись о выдаче книги.
        /// </summary>
        /// <param name="borrow">Модель выдачи с обновлёнными данными.</param>
        /// <exception cref="InvalidOperationException">Если запись не найдена.</exception>
        public async Task UpdateBorrowAsync(Borrow borrow)
        {
            var entity = await _context.BookBorrows.FirstOrDefaultAsync(b => b.Id == borrow.Id);
            if (entity == null)
                throw new InvalidOperationException("Запись не найдена");

            entity.ReturnDate = borrow.ReturnDate;
            entity.Status = borrow.Status;

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Преобразует сущность выдачи в модель.
        /// </summary>
        /// <param name="entity">Сущность выдачи.</param>
        /// <returns>Модель выдачи.</returns>
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
        /// Преобразует модель выдачи в сущность для хранения.
        /// </summary>
        /// <param name="model">Модель выдачи.</param>
        /// <returns>Сущность выдачи.</returns>
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
        /// Получает запись о выдаче книги по её идентификатору.
        /// </summary>
        /// <param name="id">Идентификатор записи.</param>
        /// <returns>Модель выдачи или null, если запись не найдена.</returns>
        public async Task<Borrow> GetByIdAsync(Guid id)
        {
            var entity = await _context.BookBorrows.FirstOrDefaultAsync(b => b.Id == id);
            return entity == null ? null : MapToModel(entity);
        }

        /// <summary>
        /// Получает последнюю (самую позднюю по дате) запись о выдаче книги для указанного читателя.
        /// </summary>
        /// <param name="readerId">Идентификатор читателя.</param>
        /// <returns>Модель выдачи или null, если записей нет.</returns>
        public async Task<Borrow> GetByReaderIdAsync(Guid readerId)
        {
            var entity = await _context.BookBorrows
                .OrderByDescending(b => b.BorrowDate)
                .FirstOrDefaultAsync(b => b.ReaderId == readerId);

            return entity == null ? null : MapToModel(entity);
        }
    }
}
