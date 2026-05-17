using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сервис отправки email‑сообщений через SMTP.
/// Инкапсулирует работу с MailKit и использует настройки приложения.
/// </summary>
public sealed class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    /// <summary>
    /// Создаёт экземпляр SMTP‑сервиса.
    /// </summary>
    public SmtpEmailService(
        IOptions<EmailSettings> settings,
        ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Отправляет email‑сообщение через SMTP.
    /// </summary>
    public async Task<EmailSendResult> SendAsync(EmailMessage message)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(new MailboxAddress(string.Empty, message.To));
        email.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = string.IsNullOrEmpty(message.HtmlBody) ? null : message.HtmlBody,
            TextBody = string.IsNullOrEmpty(message.TextBody) ? null : message.TextBody
        };

        email.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();

            var security = _settings.UseSsl
                ? SecureSocketOptions.StartTls
                : SecureSocketOptions.None;

            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, security);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email успешно отправлен: {To}", message.To);
            return EmailSendResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка отправки email: {To}", message.To);
            return EmailSendResult.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Отправляет письмо всем администраторам, указанным в настройках.
    /// </summary>
    public async Task<IEnumerable<EmailSendResult>> SendToAdminsAsync(
        string subject,
        string htmlBody,
        string textBody = null)
    {
        var results = new List<EmailSendResult>();

        foreach (var adminEmail in _settings.AdminEmails)
        {
            var message = new EmailMessage
            {
                To = adminEmail,
                Subject = subject,
                HtmlBody = htmlBody,
                TextBody = textBody
            };

            var result = await SendAsync(message);
            results.Add(result);
        }

        return results;
    }
}
