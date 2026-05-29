using RabbitMQ.Client;

namespace PracticalWork.Reports.MessageBroker;

/// <summary>
/// Канал RabbitMQ, настроенный для потребления логов активности.
/// </summary>
public interface IReportsRabbitMqChannel : IDisposable
{
    IModel Channel { get; }
}
