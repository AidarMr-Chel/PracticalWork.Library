using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace PracticalWork.Reports.MessageBroker;

/// <summary>
/// Создаёт и настраивает канал RabbitMQ для модуля отчётов.
/// </summary>
public sealed class ReportsRabbitMqChannel : IReportsRabbitMqChannel
{
    private readonly IConnection _connection;

    public ReportsRabbitMqChannel(IOptions<RabbitMqOptions> options)
    {
        var settings = options.Value;
        var factory = new ConnectionFactory
        {
            HostName = settings.Host,
            UserName = settings.User,
            Password = settings.Password
        };

        _connection = factory.CreateConnection();
        Channel = _connection.CreateModel();

        Channel.ExchangeDeclare(
            exchange: settings.Exchange,
            type: ExchangeType.Topic,
            durable: true);

        Channel.QueueDeclare(
            queue: "reports.activity",
            durable: true,
            exclusive: false,
            autoDelete: false);

        Channel.QueueBind(
            queue: "reports.activity",
            exchange: settings.Exchange,
            routingKey: "#");
    }

    /// <inheritdoc />
    public IModel Channel { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        Channel.Dispose();
        _connection.Dispose();
    }
}
