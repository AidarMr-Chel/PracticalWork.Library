using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PracticalWork.Reports.Cache.Redis;

public static class Entry
{
    /// <summary>
    /// Регистрация Redis-кэша для сервиса Reports
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
