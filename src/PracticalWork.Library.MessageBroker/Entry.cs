using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.MessageBroker.Abstractions;
using PracticalWork.Library.MessageBroker.RabbitMQ;

namespace PracticalWork.Library.MessageBroker
{
    public static class Entry
    {
        public static IServiceCollection AddRabbitMqPublisher(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqOptions>(configuration.GetSection("App:RabbitMQ"));

            services.AddSingleton<IMessagePublisher, RabbitMqMessagePublisher>();

            return services;
        }

    }
}
