using Microsoft.EntityFrameworkCore;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Data.PostgreSql.Repositories;

/// <summary>
/// Репозиторий для получения данных напоминаний о возврате книг.
/// Выполняет объединение сущностей <see cref="Borrow"/>, <see cref="Reader"/> и <see cref="Book"/>
/// за один SQL‑запрос, после чего формирует DTO <see cref="ReminderData"/>.
/// </summary>
public sealed class ReminderRepository : IReminderRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Создаёт экземпляр репозитория напоминаний.
    /// </summary>
    /// <param name="context">Контекст основной базы данных приложения.</param>
    public ReminderRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Возвращает данные для формирования email‑напоминаний по указанной дате возврата.
    /// Выполняет JOIN между выдачами книг, читателями и книгами.
    /// EF Core выполняет только SQL‑часть, а создание <see cref="ReminderData"/> происходит в памяти.
    /// </summary>
    /// <param name="targetDueDate">Дата, для которой требуется сформировать напоминания.</param>
    /// <param name="ct">Токен отмены операции.</param>
    /// <returns>
    /// Коллекция объектов <see cref="ReminderData"/>, содержащих сведения о читателе,
    /// книге и сроке возврата.
    /// </returns>
    public async Task<IEnumerable<ReminderData>> GetRemindersForDueDateAsync(
        DateOnly targetDueDate,
        CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // 1) Выполняем SQL‑запрос (JOIN)
        var raw = await _context.BookBorrows
            .AsNoTracking()
            .Where(b => b.DueDate == targetDueDate && b.Status == BookIssueStatus.Issued)
            .Join(
                _context.Readers,
                borrow => borrow.ReaderId,
                reader => reader.Id,
                (borrow, reader) => new { borrow, reader }
            )
            .Join(
                _context.Books,
                br => br.borrow.BookId,
                book => book.Id,
                (br, book) => new
                {
                    BorrowId = br.borrow.Id,
                    ReaderFullName = br.reader.FullName,
                    ReaderEmail = br.reader.Email,
                    BookAuthors = book.Authors,
                    BookTitle = book.Title,
                    DueDate = br.borrow.DueDate
                }
            )
            .ToListAsync(ct); // SQL заканчивается здесь

        // 2) Формируем DTO в памяти (EF больше не участвует)
        return raw
            .Where(x => !string.IsNullOrEmpty(x.ReaderEmail))
            .Select(x => new ReminderData(
                x.BorrowId,
                x.ReaderFullName,
                x.ReaderEmail!,
                x.BookAuthors ?? new List<string>(),
                x.BookTitle,
                x.DueDate,
                x.DueDate.DayNumber - today.DayNumber
            ))
            .ToList();
    }
}
