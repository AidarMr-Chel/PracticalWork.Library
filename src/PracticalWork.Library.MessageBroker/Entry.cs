using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.MessageBroker.Abstractions;
using PracticalWork.Library.MessageBroker.RabbitMQ;

namespace PracticalWork.Library.MessageBroker
{
    /// <summary>
    /// Точка входа для добавления зависимостей
    /// </summary>
    public static class Entry
    {
        /// <summary>
        /// Добавление зависимостей для публикации сообщений в RabbitMQ
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("App:RabbitMQ"));

            services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();

            return services;
        }

    }
}
