using System.Net.Mail;

namespace PracticalWork.Library.Abstractions.Services;

public interface IEmailService
{
    Task<EmailSendResult> SendAsync(EmailMessage message);
}