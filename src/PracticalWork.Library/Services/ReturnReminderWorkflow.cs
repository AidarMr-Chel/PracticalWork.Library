using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

/// <summary>
/// Бизнес‑воркфлоу отправки напоминаний о возврате книг.
/// Инкапсулирует всю логику сценария, отделённую от Quartz.
/// </summary>
public sealed class ReturnReminderWorkflow : IWorkflow
{
    private readonly IReminderRepository _reminderRepository;
    private readonly IEmailService _emailService;
    private readonly IEmailTemplateService _templateService;
    private readonly ReturnReminderTemplate _template;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ReturnReminderWorkflow> _logger;

    /// <summary>
    /// Создаёт экземпляр воркфлоу отправки напоминаний.
    /// </summary>
    public ReturnReminderWorkflow(
        IReminderRepository reminderRepository,
        IEmailService emailService,
        IEmailTemplateService templateService,
        IOptions<EmailTemplateSettings> templateSettings,
        TimeProvider timeProvider,
        ILogger<ReturnReminderWorkflow> logger)
    {
        _reminderRepository = reminderRepository;
        _emailService = emailService;
        _templateService = templateService;
        _template = templateSettings.Value.ReturnReminder;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    /// <summary>
    /// Выполняет процесс поиска выдач и отправки email‑напоминаний.
    /// </summary>
    public async Task<WorkflowResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(_timeProvider.GetUtcNow().DateTime);
        var targetDueDate = today.AddDays(_template.DaysBeforeDueDate);

        _logger.LogInformation("Поиск выдач со сроком возврата {TargetDate}", targetDueDate);

        var reminders = await _reminderRepository.GetRemindersForDueDateAsync(
            targetDueDate,
            cancellationToken);

        var total = reminders.Count();
        _logger.LogInformation("Найдено выдач для напоминаний: {Count}", total);

        if (total == 0)
            return new WorkflowResult(IsSuccess: true, ProcessedCount: 0, ErrorCount: 0);

        int successCount = 0;
        int errorCount = 0;

        foreach (var r in reminders)
        {
            try
            {
                var parameters = BuildTemplateParameters(r, today);

                var email = new EmailMessage
                {
                    To = r.ReaderEmail,
                    Subject = _template.SubjectTemplate.Replace("{BookTitle}", r.BookTitle),
                    HtmlBody = await _templateService.RenderAsync("ReturnReminder.html", parameters),
                    TextBody = await _templateService.RenderAsync("ReturnReminder.txt", parameters)
                };

                var result = await _emailService.SendAsync(email);

                if (result.IsSuccess)
                {
                    _logger.LogInformation(
                        "Напоминание отправлено: {Email} → {Book}",
                        r.ReaderEmail,
                        r.BookTitle);

                    successCount++;
                }
                else
                {
                    _logger.LogWarning(
                        "Ошибка отправки письма {Email}: {Error}",
                        r.ReaderEmail,
                        result.Error);

                    errorCount++;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Ошибка при обработке выдачи {BorrowId}",
                    r.BorrowId);

                errorCount++;
            }
        }

        return new WorkflowResult(
            IsSuccess: errorCount == 0,
            ProcessedCount: successCount,
            ErrorCount: errorCount);
    }

    /// <summary>
    /// Формирует параметры для подстановки в шаблон письма.
    /// </summary>
    private Dictionary<string, string> BuildTemplateParameters(ReminderData r, DateOnly today)
    {
        return new Dictionary<string, string>
        {
            ["ReaderFullName"] = r.ReaderFullName,
            ["BookTitle"] = r.BookTitle,
            ["BookAuthors"] = string.Join(", ", r.BookAuthors),
            ["DueDate"] = r.DueDate.ToString("dd.MM.yyyy"),
            ["DaysLeft"] = (r.DueDate.DayNumber - today.DayNumber).ToString(),
            ["LibraryName"] = _template.LibraryName,
            ["LibraryAddress"] = _template.LibraryAddress,
            ["LibraryPhone"] = _template.LibraryPhone,
            ["WorkingHours"] = _template.WorkingHours
        };
    }
}
