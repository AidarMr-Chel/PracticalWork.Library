using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using PracticalWork.Reports.Data.PostgreSql;
using PracticalWork.Reports.Entities;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using Microsoft.Extensions.DependencyInjection;


namespace PracticalWork.Reports.MessageBroker
{
    public class ReportsEventConsumer : BackgroundService
    {
        private readonly IServiceProvider _provider;
        private readonly RabbitMqOptions _options;
        private IConnection _connection;
        private IModel _channel;

        public ReportsEventConsumer(IServiceProvider provider, IOptions<RabbitMqOptions> options)
        {
            _provider = provider;
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
                durable: true);

            _channel.QueueDeclare(
                queue: "reports.activity",
                durable: true,
                exclusive: false,
                autoDelete: false);

            _channel.QueueBind(
                queue: "reports.activity",
                exchange: _options.Exchange,
                routingKey: "#");
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (_, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var eventType = ea.RoutingKey;

                using var scope = _provider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ReportsDbContext>();

                db.ActivityLogs.Add(new ActivityLog
                {
                    EventType = eventType,
                    Payload = json,
                    CreatedAt = DateTime.UtcNow
                });

                await db.SaveChangesAsync();
            };

            _channel.BasicConsume(
                queue: "reports.activity",
                autoAck: true,
                consumer: consumer);

            return Task.CompletedTask;
        }
    }
}
