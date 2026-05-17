namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Результат отправки email‑сообщения.
/// Используется сервисами отправки писем для передачи статуса операции.
/// </summary>
public class EmailSendResult
{
    /// <summary>
    /// Признак успешной отправки письма.
    /// </summary>
    public bool IsSuccess { get; private set; }

    /// <summary>
    /// Сообщение об ошибке, если отправка не удалась.
    /// </summary>
    public string Error { get; private set; }

    /// <summary>
    /// Создаёт успешный результат отправки письма.
    /// </summary>
    public static EmailSendResult Success() => new()
    {
        IsSuccess = true
    };

    /// <summary>
    /// Создаёт результат отправки письма с ошибкой.
    /// </summary>
    /// <param name="error">Описание ошибки.</param>
    public static EmailSendResult Failure(string error) => new()
    {
        IsSuccess = false,
        Error = error
    };
}
