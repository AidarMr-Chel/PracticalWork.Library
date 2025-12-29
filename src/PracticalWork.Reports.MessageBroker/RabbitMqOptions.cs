namespace PracticalWork.Reports.MessageBroker
{
    /// <summary>
    /// Настройки подключения к брокеру сообщений RabbitMQ.
    /// Используются для конфигурации подключения и обмена сообщениями.
    /// </summary>
    public class RabbitMqOptions
    {
        /// <summary>
        /// Адрес хоста RabbitMQ (например, localhost или адрес сервера).
        /// </summary>
        public string Host { get; set; } = default!;

        /// <summary>
        /// Имя пользователя для подключения к RabbitMQ.
        /// </summary>
        public string User { get; set; } = default!;

        /// <summary>
        /// Пароль пользователя для подключения к RabbitMQ.
        /// </summary>
        public string Password { get; set; } = default!;

        /// <summary>
        /// Имя обменника (exchange), через который отправляются сообщения.
        /// </summary>
        public string Exchange { get; set; } = default!;
    }
}
