using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PracticalWork.Reports.Cache.Redis;

public static class Entry
{
    /// <summary>
    /// Добавляет к сервисам кэширование отчетов с использованием Redis
    /// </summary>
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
