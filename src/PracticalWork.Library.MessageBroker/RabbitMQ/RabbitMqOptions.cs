namespace PracticalWork.Library.MessageBroker.RabbitMQ
{
    /// <summary>
    /// Настройки подключения к RabbitMQ.
    /// Используются для конфигурации соединения и выбора exchange.
    /// </summary>
    public class RabbitMqOptions
    {
        /// <summary>
        /// Хост RabbitMQ.
        /// </summary>
        public string Host { get; set; } = default!;

        /// <summary>
        /// Имя пользователя для подключения.
        /// </summary>
        public string User { get; set; } = default!;

        /// <summary>
        /// Пароль пользователя для подключения.
        /// </summary>
        public string Password { get; set; } = default!;

        /// <summary>
        /// Имя exchange, в который публикуются события.
        /// </summary>
        public string Exchange { get; set; } = "library.events";
    }
}
