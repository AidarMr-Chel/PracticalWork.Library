using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticalWork.Library.Data.PostgreSql.Repositories
{
    public sealed class BorrowRepository : IBorrowRepository
    {
        private readonly AppDbContext _appDbContext;
        public BorrowRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<Guid> CreateBorrow(Guid bookId, Guid readerId)
        {
            var book = await _appDbContext.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null)
                throw new InvalidOperationException($"Книга с id={bookId} не найдена");

            if (book.Status == BookStatus.Archived)
                throw new InvalidOperationException("Книга находится в архиве и недоступна для выдачи");

            if (book.Status == BookStatus.Borrow)
                throw new InvalidOperationException("Книга уже выдана другому читателю");

            var reader = await _appDbContext.Readers.FirstOrDefaultAsync(r => r.Id == readerId);
            if (reader == null)
                throw new InvalidOperationException($"Читатель с id={readerId} не найден");

            if (!reader.IsActive)
                throw new InvalidOperationException("Карточка читателя неактивна");

            var borrowEntity = new BookBorrowEntity
            {
                ReaderId = readerId,
                BookId = bookId,
                BorrowDate = DateOnly.FromDateTime(DateTime.UtcNow),
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                Status = BookIssueStatus.Issued
            };

            //todo: читателю добавить запись о том что он взял книгу

            _appDbContext.BookBorrows.Add(borrowEntity);

            book.Status = BookStatus.Borrow;
            _appDbContext.Books.Update(book);

            await _appDbContext.SaveChangesAsync();
            return borrowEntity.Id;
        }

    }
}
