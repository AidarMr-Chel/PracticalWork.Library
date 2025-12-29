using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PracticalWork.Reports.Cache.Redis;

/// <summary>
/// Точка входа для регистрации кэширования отчётов с использованием Redis.
/// Позволяет подключить Redis‑кэш и сервисы, работающие с ним.
/// </summary>
public static class Entry
{
    /// <summary>
    /// Регистрирует в контейнере сервисов кэширование отчётов на базе Redis.
    /// Настройки подключения берутся из конфигурации приложения.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <param name="configuration">Конфигурация приложения, содержащая параметры Redis.</param>
    /// <returns>Коллекция сервисов с добавленным кэшированием отчётов.</returns>
    public static IServiceCollection AddReportsCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["App:Redis:Configuration"];
            options.InstanceName = configuration["App:Redis:InstanceName"];
        });

        services.AddScoped<RedisCacheService>();

        return services;
    }
}
