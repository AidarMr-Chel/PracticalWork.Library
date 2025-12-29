using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Services;

namespace PracticalWork.Library.Cache.Redis;

public static class Entry
{
    /// <summary>
    /// Добавляет к сервисам кэширование с использованием Redis
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
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
