using PracticalWork.Library.Abstractions.Services;

public interface IEmailService
{
    /// <summary>
    /// Отправляет письмо одному получателю.
    /// </summary>
    Task<EmailSendResult> SendAsync(EmailMessage message);

    /// <summary>
    /// Отправляет письмо всем администраторам из настроек.
    /// </summary>
    /// <param name="subject">Тема письма.</param>
    /// <param name="htmlBody">HTML-тело письма.</param>
    /// <param name="textBody">Текстовое тело письма (опционально).</param>
    /// <returns>Результат отправки (успех/ошибка для каждого адреса).</returns>
    Task<IEnumerable<EmailSendResult>> SendToAdminsAsync(
        string subject,
        string htmlBody,
        string textBody = null);
}