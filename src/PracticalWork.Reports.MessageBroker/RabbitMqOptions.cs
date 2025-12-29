namespace PracticalWork.Reports.MessageBroker
{
    /// <summary>
    /// Настройки для подключения к RabbitMQ
    /// </summary>
    public class RabbitMqOptions
    {
        public string Host { get; set; } = default!;
        public string User { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Exchange { get; set; } = default!;
    }

}
