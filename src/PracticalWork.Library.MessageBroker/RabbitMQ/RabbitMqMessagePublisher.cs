using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Contracts.v1.Events;
using PracticalWork.Library.MessageBroker.Abstractions;
using RabbitMQ.Client;

namespace PracticalWork.Library.MessageBroker.RabbitMQ
{
    /// <summary>
    /// Публикатор событий в RabbitMQ.
    /// Отвечает за сериализацию событий и отправку их
    /// в указанный exchange с использованием routing key.
    /// </summary>
    public class RabbitMqMessagePublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly RabbitMqOptions _options;

        /// <summary>
        /// Создаёт новый экземпляр публикатора сообщений RabbitMQ.
        /// Устанавливает соединение и объявляет exchange.
        /// </summary>
        /// <param name="options">Настройки подключения к RabbitMQ.</param>
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
        /// Публикует событие в RabbitMQ.
        /// </summary>
        /// <param name="evt">Событие, наследующее <see cref="BaseLibraryEvent"/>.</param>
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

        /// <summary>
        /// Освобождает ресурсы соединения и канала RabbitMQ.
        /// </summary>
        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
