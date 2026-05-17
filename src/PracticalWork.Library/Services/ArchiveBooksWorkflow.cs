using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

/// <summary>
/// Бизнес‑воркфлоу архивации неактивных книг.
/// Инкапсулирует всю логику сценария, отделённую от Quartz.
/// </summary>
public sealed class ArchiveBooksWorkflow : IWorkflow
{
    private readonly IBookService _bookService;
    private readonly IBookRepository _bookRepository;
    private readonly IMinioService _minioService;
    private readonly ArchiveSettings _settings;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ArchiveBooksWorkflow> _logger;

    /// <summary>
    /// Создаёт экземпляр воркфлоу архивации книг.
    /// </summary>
    public ArchiveBooksWorkflow(
        IBookService bookService,
        IBookRepository bookRepository,
        IMinioService minioService,
        IOptions<ArchiveSettings> settings,
        TimeProvider timeProvider,
        ILogger<ArchiveBooksWorkflow> logger)
    {
        _bookService = bookService;
        _bookRepository = bookRepository;
        _minioService = minioService;
        _settings = settings.Value;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    /// <summary>
    /// Выполняет процесс архивации книг, не выдававшихся указанное количество лет.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат выполнения воркфлоу.</returns>
    public async Task<WorkflowResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "ArchiveBooksWorkflow: параметры — лет без выдачи={Years}, лимит книг={Limit}",
            _settings.YearsWithoutBorrow,
            _settings.MaxBooksPerRun);

        var booksToArchive = await _bookRepository.GetArchivableBooksAsync(
            _settings.YearsWithoutBorrow,
            _settings.MaxBooksPerRun,
            cancellationToken);

        var total = booksToArchive.Count();
        _logger.LogInformation("Найдено книг для архивации: {Count}", total);

        if (total == 0)
            return new WorkflowResult(IsSuccess: true, ProcessedCount: 0, ErrorCount: 0);

        int successCount = 0;
        int errorCount = 0;

        foreach (var book in booksToArchive)
        {
            try
            {
                // Бизнес‑логика: архивация книги
                await _bookService.ArchivingBook(book.Id);
                _logger.LogDebug("Книга {BookId} архивирована", book.Id);

                // Инфраструктурная операция: удаление обложки (не критично)
                if (!string.IsNullOrEmpty(book.CoverImagePath))
                {
                    try
                    {
                        await _minioService.DeleteAsync(book.CoverImagePath);
                        _logger.LogDebug("Обложка удалена: {CoverPath}", book.CoverImagePath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(
                            ex,
                            "Не удалось удалить обложку {CoverPath}",
                            book.CoverImagePath);
                    }
                }

                successCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при архивации книги {BookId}", book.Id);
                errorCount++;
            }
        }

        return new WorkflowResult(
            IsSuccess: errorCount == 0,
            ProcessedCount: successCount,
            ErrorCount: errorCount);
    }
}
