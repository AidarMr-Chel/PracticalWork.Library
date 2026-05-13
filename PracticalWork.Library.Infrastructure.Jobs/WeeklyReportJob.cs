using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Contracts.v1.Events.Borrows;
using PracticalWork.Library.Data.PostgreSql;
using PracticalWork.Library.Data.PostgreSql.Entities;
using PracticalWork.Library.Models;
using Quartz;

namespace PracticalWork.Library.Infrastructure.Jobs;

public class WeeklyReportJob : IJob
{
    private readonly ReportsDbContext _reportsDbContext;
    private readonly IMinioService _minioService;
    private readonly IEmailService _emailService;
    private readonly EmailSettings _emailSettings;
    private readonly MinioReportsSettings _reportsSettings;
    private readonly ILogger<WeeklyReportJob> _logger;

    public WeeklyReportJob(
        ReportsDbContext reportsDbContext,
        IMinioService minioService,
        IEmailService emailService,
        IOptions<EmailSettings> emailSettings,
        IOptions<MinioReportsSettings> reportsSettings,
        ILogger<WeeklyReportJob> logger)
    {
        _reportsDbContext = reportsDbContext;
        _minioService = minioService;
        _emailService = emailService;
        _emailSettings = emailSettings.Value;
        _reportsSettings = reportsSettings.Value;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("--- 🚀 Начало WeeklyReportJob ---");

        var lastWeekStart = GetLastWeekStart();
        var lastWeekEnd = lastWeekStart.AddDays(7).AddTicks(-1);

        _logger.LogInformation("📅 Период отчета: {Start} — {End}",
            lastWeekStart.ToString("yyyy-MM-dd"), lastWeekEnd.ToString("yyyy-MM-dd"));

        var events = await _reportsDbContext.ActivityLogs
            .Where(e => e.CreatedAt >= lastWeekStart && e.CreatedAt <= lastWeekEnd)
            .ToListAsync();

        _logger.LogInformation("📦 Найдено событий в БД: {Count}", events.Count);

        if (!events.Any())
        {
            _logger.LogWarning("⚠ Нет данных для отчета за неделю {Week}", lastWeekStart.ToString("yyyy-MM-dd"));
            return;
        }

        var stats = AggregateStatistics(events, lastWeekStart, lastWeekEnd);

        var csvBytes = GenerateCsv(stats, lastWeekStart, lastWeekEnd);
        var fileName = $"weekly_report_{lastWeekStart:yyyy-MM-dd}.csv";

        using var csvStream = new MemoryStream(csvBytes);

        await _minioService.UploadToBucketAsync(
            stream: csvStream,
            objectName: fileName,
            contentType: "text/csv; charset=utf-8",
            bucketName: _reportsSettings.Bucket);

        _logger.LogInformation("📁 Файл загружен в бакет '{Bucket}': {FileName}",
            _reportsSettings.Bucket, fileName);

        var expiry = TimeSpan.FromSeconds(_reportsSettings.PresignedUrlExpirySeconds);
        var fileUrl = await _minioService.GetPresignedUrlAsync(
            objectName: fileName,
            expiry: expiry,
            bucketName: _reportsSettings.Bucket);

        _logger.LogInformation("🔗 Presigned-ссылка (действует {Expiry}): {Url}",
            expiry, fileUrl);

        foreach (var adminEmail in _emailSettings.AdminEmails)
        {
            var email = new EmailMessage
            {
                To = adminEmail,
                Subject = $"📊 Еженедельный отчет библиотеки ({lastWeekStart:dd.MM} - {lastWeekEnd.AddDays(1):dd.MM})",
                HtmlBody = GenerateHtmlReport(stats, fileUrl, lastWeekStart, lastWeekEnd),
                TextBody = $"Отчет доступен по ссылке (действует {expiry.Days} дн.): {fileUrl}"
            };

            var result = await _emailService.SendAsync(email);

            _logger.LogInformation("📧 Письмо админу {Email}: {Status}",
                adminEmail, result.IsSuccess ? "OK" : result.Error);
        }

        _logger.LogInformation("--- ✅ WeeklyReportJob завершен успешно ---");
    }

    private static DateTime GetLastWeekStart()
    {
        var today = DateTime.UtcNow.Date;
        var dayOfWeek = (int)today.DayOfWeek;
        var daysToSubtract = dayOfWeek == 0 ? 6 : dayOfWeek - 1;
        var thisMonday = today.AddDays(-daysToSubtract);
        return thisMonday.AddDays(-7); 
    }


    private WeeklyStats AggregateStatistics(IEnumerable<ActivityLog> events, DateTime start, DateTime end)
    {
        int newBorrows = 0, returns = 0, overdue = 0;

        foreach (var e in events)
        {
            try
            {
                switch (e.EventType)
                {
                    case "book.borrowed":
                        var borrowed = JsonSerializer.Deserialize<BookBorrowedEvent>(e.Payload);
                        if (borrowed?.BorrowedAt >= start && borrowed?.BorrowedAt <= end)
                            newBorrows++;
                        break;

                    case "book.returned":
                        var returned = JsonSerializer.Deserialize<BookReturnedEvent>(e.Payload);
                        if (returned?.ReturnedAt >= start && returned?.ReturnedAt <= end)
                        {
                            returns++;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Ошибка десериализации или агрегации события {EventType}", e.EventType);
            }
        }

        return new WeeklyStats
        {
            NewBorrowsCount = newBorrows,
            ReturnsCount = returns,
            OverdueCount = overdue
        };
    }


    private static byte[] GenerateCsv(WeeklyStats stats, DateTime start, DateTime end)
    {
        var bom = new byte[] { 0xEF, 0xBB, 0xBF };

        var lines = new[]
        {
            "Параметр;Значение",
            $"Период;{start:dd.MM.yyyy} - {end.AddDays(1):dd.MM.yyyy}",
            $"Новых выдач;{stats.NewBorrowsCount}",
            $"Возвратов;{stats.ReturnsCount}",
            $"Просроченных возвратов;{stats.OverdueCount}"
        };

        var csvContent = string.Join("\n", lines);
        return bom.Concat(Encoding.UTF8.GetBytes(csvContent)).ToArray();
    }


    private static string GenerateHtmlReport(WeeklyStats stats, string fileUrl, DateTime start, DateTime end)
    {
        return $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; color: #333; font-size: 16px; margin: 20px;'>
    <h2 style='color: #2c3e50; margin-bottom: 20px;'>📊 Еженедельный отчет библиотеки</h2>
    
    <p style='margin: 0 0 20px;'><strong>Период:</strong> {start:dd.MM.yyyy} — {end.AddDays(1):dd.MM.yyyy}</p>
    
    <table style='border-collapse: collapse; width: 100%; margin-bottom: 20px;'>
        <thead>
            <tr style='background: #f4f4f4; text-align: left;'>
                <th style='border: 1px solid #ddd; padding: 12px;'>Параметр</th>
                <th style='border: 1px solid #ddd; padding: 12px;'>Значение</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td style='border: 1px solid #ddd; padding: 12px;'>Новых выдач</td>
                <td style='border: 1px solid #ddd; padding: 12px; text-align: center; font-weight: bold;'>{stats.NewBorrowsCount}</td>
            </tr>
            <tr>
                <td style='border: 1px solid #ddd; padding: 12px;'>Возвратов</td>
                <td style='border: 1px solid #ddd; padding: 12px; text-align: center; font-weight: bold;'>{stats.ReturnsCount}</td>
            </tr>
            <tr>
                <td style='border: 1px solid #ddd; padding: 12px;'>Просроченных возвратов</td>
                <td style='border: 1px solid #ddd; padding: 12px; text-align: center; font-weight: bold; color: #dc3545;'>{stats.OverdueCount}</td>
            </tr>
        </tbody>
    </table>
    
    <p style='margin: 0 0 20px;'>
        <a href='{fileUrl}' style='background: #007bff; color: white; padding: 10px 20px; 
            text-decoration: none; border-radius: 4px; display: inline-block;'>
            📥 Скачать полный CSV-отчет
        </a>
    </p>
    
    <hr style='border: none; border-top: 1px solid #eee; margin: 30px 0;'/>
    
    <p style='color: #6c757d; font-size: 14px; margin: 0;'>
        Отчет сгенерирован автоматически.<br/>
        Ссылка для скачивания действительна 7 дней. Файл хранится в хранилище 90 дней.
    </p>
</body>
</html>";
    }
}


public class WeeklyStats
{
    public int NewBorrowsCount { get; set; }
    public int ReturnsCount { get; set; }
    public int OverdueCount { get; set; }
}