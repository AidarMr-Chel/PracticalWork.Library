using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PracticalWork.Reports.Services.Abstractions;
using RabbitMQ.Client.Events;
using System.Text;

namespace PracticalWork.Reports.MessageBroker;

/// <summary>
/// Потребитель событий RabbitMQ для модуля отчётов.
/// </summary>
public sealed class ReportsEventConsumer : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IReportsRabbitMqChannel _rabbitMq;
    private readonly ILogger<ReportsEventConsumer> _logger;

    public ReportsEventConsumer(
        IServiceProvider provider,
        IReportsRabbitMqChannel rabbitMq,
        ILogger<ReportsEventConsumer> logger)
    {
        _provider = provider;
        _rabbitMq = rabbitMq;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var channel = _rabbitMq.Channel;
        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.Received += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var eventType = ea.RoutingKey;

                using var scope = _provider.CreateScope();
                var ingestion = scope.ServiceProvider.GetRequiredService<IActivityLogIngestionService>();

                await ingestion.IngestAsync(eventType, json, stoppingToken);

                channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to ingest activity event");
                channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        channel.BasicConsume(
            queue: "reports.activity",
            autoAck: false,
            consumerTag: string.Empty,
            noLocal: false,
            exclusive: false,
            arguments: null,
            consumer: consumer);

        return Task.CompletedTask;
    }
}
