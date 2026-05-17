namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Модель сообщения электронной почты.
/// Используется сервисами отправки писем для передачи данных письма.
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// Адрес получателя письма.
    /// </summary>
    public string To { get; set; } = string.Empty;

    /// <summary>
    /// Тема письма.
    /// </summary>
    public string Subject { get; set; } = string.Empty;

    /// <summary>
    /// HTML‑версия тела письма.
    /// </summary>
    public string HtmlBody { get; set; } = string.Empty;

    /// <summary>
    /// Текстовая версия тела письма (fallback для клиентов без HTML).
    /// </summary>
    public string TextBody { get; set; } = string.Empty;
}
