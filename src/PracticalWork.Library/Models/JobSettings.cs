namespace PracticalWork.Library.Models;

public class EmailSettings
{
    public string SmtpServer { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 25;
    public bool UseSsl { get; set; } = false;
    public string SenderName { get; set; } = "Библиотека";
    public string SenderEmail { get; set; } = "noreply@library.local";
    public List<string> AdminEmails { get; set; } = new();
}

public class JobSettings
{
    public Dictionary<string, JobConfiguration> Jobs { get; set; } = new();
}

public class JobConfiguration
{
    public string CronExpression { get; set; } = string.Empty;
    public int MaxRetries { get; set; } = 3;
    public int TimeoutMinutes { get; set; } = 30;
}

public class ArchiveSettings
{
    public int YearsWithoutBorrow { get; set; } = 3;
    public int MaxBooksPerRun { get; set; } = 100;
}

public class EmailTemplateSettings
{
    public ReturnReminderTemplate ReturnReminder { get; set; } = new();
    public WeeklyReportTemplate WeeklyReport { get; set; } = new();
}

public class ReturnReminderTemplate
{
    public string SubjectTemplate { get; set; } = "Напоминание о возврате книги:\"{BookTitle}\"";
    public int DaysBeforeDueDate { get; set; } = 3;
    public string LibraryName { get; set; } = "Библиотека";
    public string LibraryAddress { get; set; } = "";
    public string LibraryPhone { get; set; } = "";
    public string WorkingHours { get; set; } = "";
}

public class WeeklyReportTemplate
{
    public string SubjectTemplate { get; set; } = "Еженедельный отчет библиотеки за период{StartDate}-{EndDate}";
    public string[] AdminEmails { get; set; } = Array.Empty<string>();
    public int ReportRetentionDays { get; set; } = 90;
}