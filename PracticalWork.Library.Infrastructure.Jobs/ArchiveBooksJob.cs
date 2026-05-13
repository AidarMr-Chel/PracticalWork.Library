using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Contracts.v1.Events.Books;
using PracticalWork.Library.Enums;
using PracticalWork.Library.MessageBroker.Abstractions;
using PracticalWork.Library.Models; 
using Quartz;

namespace PracticalWork.Library.Infrastructure.Jobs;

public class ArchiveBooksJob : IJob
{
    private readonly IBookRepository _bookRepository;
    private readonly IMinioService _minioService;
    private readonly IMessagePublisher _publisher;
    private readonly ArchiveSettings _settings;
    private readonly ILogger<ArchiveBooksJob> _logger;

    public ArchiveBooksJob(
        IBookRepository bookRepository,
        IMinioService minioService,
        IMessagePublisher publisher,
        IOptions<ArchiveSettings> settings,
        ILogger<ArchiveBooksJob> logger)
    {
        _bookRepository = bookRepository;
        _minioService = minioService;
        _publisher = publisher;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("--- 🚀 Начало ArchiveBooksJob ---");

        var booksToArchive = await _bookRepository.GetArchivableBooksAsync(
            _settings.YearsWithoutBorrow,
            _settings.MaxBooksPerRun,
            context.CancellationToken);

        _logger.LogInformation("🔍 Найдено книг для архивации: {Count}", booksToArchive.Count());

        if (!booksToArchive.Any())
        {
            _logger.LogInformation("✅ Нет книг для архивации в этом запуске");
            return;
        }

        int successCount = 0, failedCount = 0;

        foreach (var book in booksToArchive)
        {
            try
            {
                _logger.LogDebug("📚 Архивируем книгу: {BookId} - {Title}", book.Id, book.Title);

                book.Status = BookStatus.Archived;

                await _bookRepository.UpdateAsync(book);

                await _publisher.PublishAsync(new BookArchivedEvent
                {
                    BookId = book.Id,
                    ArchivedAt = DateTime.UtcNow
                });

                _logger.LogDebug("📤 Событие book.archived опубликовано для {BookId}", book.Id);

                if (!string.IsNullOrEmpty(book.CoverImagePath))
                {
                    await _minioService.DeleteAsync(book.CoverImagePath);
                    _logger.LogDebug("🗑️ Обложка удалена: {CoverPath}", book.CoverImagePath);
                }

                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Ошибка при архивации книги {BookId}", book.Id);
                failedCount++;
            }
        }

        _logger.LogInformation("--- ✅ ArchiveBooksJob завершен: Успешно {Success}, Ошибки {Failed} ---",
            successCount, failedCount);
    }
}