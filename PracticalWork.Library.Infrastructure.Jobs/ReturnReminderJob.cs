using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Enums;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Infrastructure.Jobs;

public class ReturnReminderJob : IJob
{
    private readonly IEmailService _emailService;
    private readonly IBorrowRepository _borrowRepository;
    private readonly IReaderRepository _readerRepository;
    private readonly IBookRepository _bookRepository;
    private readonly INotificationLogRepository _notificationLogRepository;
    private readonly ReturnReminderTemplate _template;
    private readonly ILogger<ReturnReminderJob> _logger;

    public ReturnReminderJob(
        IEmailService emailService,
        IBorrowRepository borrowRepository,
        IReaderRepository readerRepository,
        IBookRepository bookRepository,
        INotificationLogRepository notificationLogRepository,
        IOptions<EmailTemplateSettings> templateSettings,
        ILogger<ReturnReminderJob> logger)
    {
        _emailService = emailService;
        _borrowRepository = borrowRepository;
        _readerRepository = readerRepository;
        _bookRepository = bookRepository;
        _notificationLogRepository = notificationLogRepository;
        _template = templateSettings.Value.ReturnReminder;
        _logger = logger;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("--- Начало выполнения ReturnReminderJob ---");

        var targetDueDate = DateOnly.FromDateTime(DateTime.UtcNow)
            .AddDays(_template.DaysBeforeDueDate);

        var twentyFourHoursAgo = DateTime.UtcNow.AddHours(-24);

        var allBorrows = await _borrowRepository.GetAllActiveAsync();

        var eligibleBorrows = allBorrows
            .Where(b => b.DueDate == targetDueDate && b.Status == BookIssueStatus.Issued)
            .ToList();

        int successCount = 0, failedCount = 0;

        foreach (var borrow in eligibleBorrows)
        {
            try
            {
                var wasNotified = await _notificationLogRepository
                    .WasNotifiedRecentlyAsync(borrow.Id, "Reminder", TimeSpan.FromHours(24));

                if (wasNotified)
                {
                    _logger.LogDebug("Пропуск выдачи {BorrowId}: напоминание уже отправлено", borrow.Id);
                    continue;
                }

                var reader = await _readerRepository.GetByIdAsync(borrow.ReaderId);
                var book = await _bookRepository.GetByIdAsync(borrow.BookId);

                if (reader == null || book == null || string.IsNullOrEmpty(reader.Email))
                {
                    _logger.LogWarning("Пропуск: нет данных для выдачи {BorrowId}", borrow.Id);
                    continue;
                }

                var email = new EmailMessage
                {
                    To = reader.Email,
                    Subject = _template.SubjectTemplate.Replace("{BookTitle}", book.Title),
                    HtmlBody = GenerateReminderHtml(reader, book, borrow),
                    TextBody = GenerateReminderText(reader, book, borrow)
                };

                var result = await _emailService.SendAsync(email);

                await _notificationLogRepository.AddAsync(
                    "Reminder",
                    borrow.Id,
                    result.IsSuccess ? "Sent" : "Failed",
                    result.IsSuccess ? null : result.Error);

                if (result.IsSuccess) successCount++;
                else failedCount++;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обработке выдачи {BorrowId}", borrow.Id);
                failedCount++;
            }
        }

        await _notificationLogRepository.SaveChangesAsync();
        _logger.LogInformation("--- Завершение: Успешно {Success}, Ошибки {Failed} ---", successCount, failedCount);
    }

    #region Шаблоны писем (строго по ТЗ)

    private string GenerateReminderHtml(Reader reader, Book book, Borrow borrow)
    {
        var daysLeft = borrow.DueDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;

        return $@"
    <!DOCTYPE html>
    <html>
    <head><meta charset='utf-8'></head>
    <body style='font-family: Arial; color: #333333; font-size: 16px; margin: 0; padding: 20px;'>
        <p style='margin: 0 0 20px;'>Уважаемый(ая) {reader.FullName}!</p>
        <p style='margin: 0 0 20px;'>Напоминаем вам о необходимости возврата книги в библиотеку.</p>

        <div style='border: 1px solid #dee2e6; background: #ffffff; padding: 20px; margin-bottom: 20px;'>
            <h3 style='font-family: Arial; font-size: 18px; color: #2c3e50; font-weight: bold; margin: 0 0 15px;'>
                ИНФОРМАЦИЯ О КНИГЕ:
            </h3>
            <div style='margin-bottom: 8px;'>
                <strong style='color: #495057;'>Название:</strong> 
                <span style='color: #6c757d;'>{book.Title}</span>
            </div>
            <div style='margin-bottom: 8px;'>
                <strong style='color: #495057;'>Автор(ы):</strong> 
                <span style='color: #6c757d;'>{string.Join(", ", book.Authors)}</span>
            </div>
            <div style='margin-bottom: 8px;'>
                <strong style='color: #495057;'>Срок возврата:</strong> 
                <span style='color: #28a745; font-weight: bold;'>{borrow.DueDate:dd.MM.yyyy}</span>
            </div>
            <div>
                <strong style='color: #495057;'>Осталось дней:</strong> 
                <span style='color: #6c757d;'>{daysLeft}</span>
            </div>
        </div>

        <p style='margin: 0 0 20px;'>Пожалуйста, верните книгу до указанной даты.</p>

        <div style='margin-bottom: 20px;'>
            <strong style='color: #2c3e50;'>КОНТАКТЫ БИБЛИОТЕКИ:</strong>
            <ul style='list-style: none; padding: 0; margin: 10px 0 0;'>
                <li style='color: #495057; font-size: 14px; margin: 5px 0;'>
                    <span style='color: #007bff;'>•</span> Адрес: {_template.LibraryAddress}
                </li>
                <li style='color: #495057; font-size: 14px; margin: 5px 0;'>
                    <span style='color: #007bff;'>•</span> Телефон: {_template.LibraryPhone}
                </li>
                <li style='color: #495057; font-size: 14px; margin: 5px 0;'>
                    <span style='color: #007bff;'>•</span> Часы работы: {_template.WorkingHours}
                </li>
            </ul>
        </div>

        <p style='font-style: italic; color: #6c757d; font-size: 14px; margin: 0 0 20px;'>
            С уважением, Администрация библиотеки
        </p>

        <hr style='border: none; border-top: 1px solid #dee2e6; margin: 20px 0;'/>
        <p style='text-align: center; color: #adb5bd; font-size: 12px; margin: 0;'>
            Это письмо было отправлено автоматически.<br/>
            Если вы уже вернули книгу, проигнорируйте это сообщение.
        </p>
    </body>
    </html>";
    }

    private string GenerateReminderText(Reader reader, Book book, Borrow borrow)
    {
        var daysLeft = borrow.DueDate.DayNumber - DateOnly.FromDateTime(DateTime.UtcNow).DayNumber;

        return $@"Уважаемый(ая) {reader.FullName}!
Напоминаем вам о необходимости возврата книги в библиотеку.

ИНФОРМАЦИЯ О КНИГЕ:
Название: {book.Title}
Автор(ы): {string.Join(", ", book.Authors)}
Срок возврата: {borrow.DueDate:dd.MM.yyyy}
Осталось дней: {daysLeft}

Пожалуйста, верните книгу до указанной даты.

КОНТАКТЫ БИБЛИОТЕКИ:
• Адрес: {_template.LibraryAddress}
• Телефон: {_template.LibraryPhone}
• Часы работы: {_template.WorkingHours}

С уважением, Администрация библиотеки
---
Это письмо было отправлено автоматически.
Если вы уже вернули книгу, проигнорируйте это сообщение.";
    }

    #endregion
}