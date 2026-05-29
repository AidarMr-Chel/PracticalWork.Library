using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PracticalWork.Reports.MessageBroker;

/// <summary>
/// Регистрация RabbitMQ для модуля отчётов.
/// </summary>
public static class Entry
{
    public static IServiceCollection AddReportsMessageBroker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<RabbitMqOptions>(configuration.GetSection("App:RabbitMQ"));
        services.AddSingleton<IReportsRabbitMqChannel, ReportsRabbitMqChannel>();
        services.AddHostedService<ReportsEventConsumer>();

        return services;
    }
}
