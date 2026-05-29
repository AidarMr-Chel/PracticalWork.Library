using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace PracticalWork.Library.MessageBroker.RabbitMQ;

/// <summary>
/// Singleton-подключение к RabbitMQ.
/// </summary>
public sealed class RabbitMqConnection : IRabbitMqConnection
{
    private readonly IConnection _connection;

    public RabbitMqConnection(IOptions<RabbitMqOptions> options)
    {
        var settings = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            UserName = settings.User,
            Password = settings.Password
        };

        _connection = factory.CreateConnection();
    }

    /// <inheritdoc />
    public IModel CreateChannel() => _connection.CreateModel();

    /// <inheritdoc />
    public void Dispose() => _connection.Dispose();
}
