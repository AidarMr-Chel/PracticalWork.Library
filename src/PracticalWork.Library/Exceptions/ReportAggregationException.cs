namespace PracticalWork.Library.Exceptions;

/// <summary>
/// Исключение, возникающее при ошибке агрегации статистики для отчётов.
/// Содержит дополнительную информацию о типе события и его полезной нагрузке.
/// </summary>
public class ReportAggregationException : Exception
{
    /// <summary>
    /// Тип события, при обработке которого произошла ошибка.
    /// </summary>
    public string EventType { get; }

    /// <summary>
    /// Сырые данные события (payload), которые не удалось обработать.
    /// </summary>
    public string Payload { get; }

    /// <summary>
    /// Создаёт исключение, связанное с ошибкой агрегации отчётных данных.
    /// </summary>
    /// <param name="eventType">Тип события, вызвавшего ошибку.</param>
    /// <param name="payload">Сырые данные события.</param>
    /// <param name="message">Сообщение об ошибке.</param>
    /// <param name="innerException">Внутреннее исключение.</param>
    public ReportAggregationException(
        string eventType,
        string payload = null,
        string message = null,
        Exception innerException = null)
        : base(message ?? $"Failed to aggregate event of type '{eventType}'", innerException)
    {
        EventType = eventType;
        Payload = payload;
    }

    /// <summary>
    /// Возвращает строковое представление исключения,
    /// включая полезную нагрузку события, если она присутствует.
    /// </summary>
    public override string ToString()
    {
        var baseStr = base.ToString();
        return string.IsNullOrEmpty(Payload)
            ? baseStr
            : $"{baseStr}\nPayload: {Payload}";
    }
}
