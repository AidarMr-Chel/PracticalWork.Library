using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Messaging;
using PracticalWork.Library.Contracts.v1.Events;
using RabbitMQ.Client;

namespace PracticalWork.Library.MessageBroker.RabbitMQ;

/// <summary>
/// Публикатор событий в RabbitMQ.
/// </summary>
public sealed class RabbitMqMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IModel _channel;
    private readonly RabbitMqOptions _options;

    public RabbitMqMessagePublisher(IRabbitMqConnection connection, IOptions<RabbitMqOptions> options)
    {
        _options = options.Value;
        _channel = connection.CreateChannel();

        _channel.ExchangeDeclare(
            exchange: _options.Exchange,
            type: ExchangeType.Topic,
            durable: true);
    }

    /// <inheritdoc />
    public Task PublishAsync(BaseLibraryEvent evt)
    {
        var json = JsonSerializer.Serialize(evt);
        var body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: _options.Exchange,
            routingKey: evt.EventType,
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose() => _channel.Dispose();
}
