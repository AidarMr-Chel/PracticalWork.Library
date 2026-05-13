using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IOptions<EmailSettings> settings, ILogger<SmtpEmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task<EmailSendResult> SendAsync(EmailMessage message)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(new MailboxAddress("", message.To));
        email.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder();
        if (!string.IsNullOrEmpty(message.HtmlBody))
            bodyBuilder.HtmlBody = message.HtmlBody;
        if (!string.IsNullOrEmpty(message.TextBody))
            bodyBuilder.TextBody = message.TextBody;

        email.Body = bodyBuilder.ToMessageBody();

        try
        {
            using var client = new SmtpClient();
            var security = _settings.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

            await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, security);
            await client.SendAsync(email);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent to {To}", message.To);
            return EmailSendResult.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", message.To);
            return EmailSendResult.Failure(ex.Message);
        }
    }
}