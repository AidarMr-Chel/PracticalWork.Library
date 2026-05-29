using RabbitMQ.Client;

namespace PracticalWork.Library.MessageBroker.RabbitMQ;

/// <summary>
/// Управляет подключением к RabbitMQ для приложения библиотеки.
/// </summary>
public interface IRabbitMqConnection : IDisposable
{
    IModel CreateChannel();
}
