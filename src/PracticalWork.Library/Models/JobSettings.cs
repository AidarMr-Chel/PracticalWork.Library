namespace PracticalWork.Library.Models;

/// <summary>
/// Настройки SMTP‑отправки писем.
/// Используются сервисом EmailService.
/// </summary>
public class EmailSettings
{
    /// <summary>
    /// Адрес SMTP‑сервера.
    /// </summary>
    public string SmtpServer { get; set; } = "localhost";

    /// <summary>
    /// Порт SMTP‑сервера.
    /// </summary>
    public int SmtpPort { get; set; } = 25;

    /// <summary>
    /// Использовать ли SSL при отправке.
    /// </summary>
    public bool UseSsl { get; set; } = false;

    /// <summary>
    /// Имя отправителя, отображаемое в письме.
    /// </summary>
    public string SenderName { get; set; } = "Библиотека";

    /// <summary>
    /// Email отправителя.
    /// </summary>
    public string SenderEmail { get; set; } = "noreply@library.local";

    /// <summary>
    /// Email‑адреса администраторов, получающих системные уведомления.
    /// </summary>
    public List<string> AdminEmails { get; set; } = new();
}

/// <summary>
/// Настройки фоновых задач (Quartz).
/// </summary>
public class JobSettings
{
    /// <summary>
    /// Конфигурации задач по ключу (имя задачи).
    /// </summary>
    public Dictionary<string, JobConfiguration> Jobs { get; set; } = new();
}

/// <summary>
/// Конфигурация отдельной фоновой задачи.
/// </summary>
public class JobConfiguration
{
    /// <summary>
    /// CRON‑выражение для расписания.
    /// </summary>
    public string CronExpression { get; set; } = string.Empty;

    /// <summary>
    /// Максимальное количество повторных попыток.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Таймаут выполнения задачи в минутах.
    /// </summary>
    public int TimeoutMinutes { get; set; } = 30;
}

/// <summary>
/// Настройки архивации книг.
/// </summary>
public class ArchiveSettings
{
    /// <summary>
    /// Количество лет без выдач, после которых книга считается архивной.
    /// </summary>
    public int YearsWithoutBorrow { get; set; } = 3;

    /// <summary>
    /// Максимальное количество книг, обрабатываемых за один запуск.
    /// </summary>
    public int MaxBooksPerRun { get; set; } = 100;
}

/// <summary>
/// Настройки шаблонов email‑уведомлений.
/// </summary>
public class EmailTemplateSettings
{
    /// <summary>
    /// Шаблон письма‑напоминания о возврате.
    /// </summary>
    public ReturnReminderTemplate ReturnReminder { get; set; } = new();

    /// <summary>
    /// Шаблон еженедельного отчёта.
    /// </summary>
    public WeeklyReportTemplate WeeklyReport { get; set; } = new();
}

/// <summary>
/// Шаблон письма‑напоминания о скором возврате книги.
/// </summary>
public class ReturnReminderTemplate
{
    /// <summary>
    /// Шаблон темы письма.
    /// </summary>
    public string SubjectTemplate { get; set; } =
        "Напоминание о возврате книги:\"{BookTitle}\"";

    /// <summary>
    /// За сколько дней до срока отправлять напоминание.
    /// </summary>
    public int DaysBeforeDueDate { get; set; } = 3;

    /// <summary>
    /// Название библиотеки.
    /// </summary>
    public string LibraryName { get; set; } = "Библиотека";

    /// <summary>
    /// Адрес библиотеки.
    /// </summary>
    public string LibraryAddress { get; set; } = string.Empty;

    /// <summary>
    /// Телефон библиотеки.
    /// </summary>
    public string LibraryPhone { get; set; } = string.Empty;

    /// <summary>
    /// Часы работы библиотеки.
    /// </summary>
    public string WorkingHours { get; set; } = string.Empty;
}

/// <summary>
/// Шаблон письма еженедельного отчёта.
/// </summary>
public class WeeklyReportTemplate
{
    /// <summary>
    /// Шаблон темы письма.
    /// </summary>
    public string SubjectTemplate { get; set; } =
        "Еженедельный отчет библиотеки за период {StartDate}-{EndDate}";

    /// <summary>
    /// Email‑адреса администраторов, получающих отчёт.
    /// </summary>
    public string[] AdminEmails { get; set; } = Array.Empty<string>();

    /// <summary>
    /// Сколько дней хранить отчёты.
    /// </summary>
    public int ReportRetentionDays { get; set; } = 90;
}
