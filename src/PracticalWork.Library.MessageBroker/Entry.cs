using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.MessageBroker.Abstractions;
using PracticalWork.Library.MessageBroker.RabbitMQ;

namespace PracticalWork.Library.MessageBroker
{
    /// <summary>
    /// Точка входа для регистрации зависимостей,
    /// связанных с публикацией событий в RabbitMQ.
    /// </summary>
    public static class Entry
    {
        /// <summary>
        /// Регистрирует публикатор сообщений RabbitMQ и загружает настройки подключения.
        /// </summary>
        /// <param name="services">Коллекция сервисов DI.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <returns>Коллекция сервисов DI для цепочки вызовов.</returns>
        public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("App:RabbitMQ"));

            services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();

            return services;
        }
    }
}
