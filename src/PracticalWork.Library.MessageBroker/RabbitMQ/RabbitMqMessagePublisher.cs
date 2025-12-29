using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Contracts.v1.Events;
using PracticalWork.Library.MessageBroker.Abstractions;
using RabbitMQ.Client;

namespace PracticalWork.Library.MessageBroker.RabbitMQ;

/// <summary>
/// Публикатор событий в RabbitMQ.
/// </summary>
public class RabbitMqMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly RabbitMqOptions _options;

    public RabbitMqMessagePublisher(IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;

        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            UserName = _options.User,
            Password = _options.Password
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: _options.Exchange,
            type: ExchangeType.Topic,
            durable: true
        );
    }

    /// <summary>
    /// Публикация события в RabbitMQ.
    /// </summary>
    /// <param name="evt">Событие, наследующее BaseLibraryEvent</param>
    public Task PublishAsync(BaseLibraryEvent evt)
    {
        var routingKey = evt.EventType;

        var json = JsonSerializer.Serialize(evt);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: _options.Exchange,
            routingKey: routingKey,
            basicProperties: null,
            body: body
        );

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
