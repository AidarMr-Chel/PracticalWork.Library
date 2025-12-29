using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Services;

namespace PracticalWork.Library.Cache.Redis;

public static class Entry
{
    /// <summary>
    /// Регистрирует в контейнере сервисов кэширование с использованием Redis.
    /// Настройки подключения берутся из IConfiguration.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <param name="configuration">Конфигурация приложения, содержащая параметры Redis.</param>
    /// <returns>
    /// Коллекция сервисов с добавленной поддержкой Redis-кэширования.
    /// </returns>
    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration["App:Redis:Configuration"];
            options.InstanceName = configuration["App:Redis:InstanceName"];
        });

        services.AddScoped<ICacheService, RedisCacheService>();

        return services;
    }
}
