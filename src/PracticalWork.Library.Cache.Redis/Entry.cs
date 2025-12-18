using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Abstractions.Services;

namespace PracticalWork.Library.Cache.Redis;

public static class Entry
{
    /// <summary>
    /// Регистрация зависимостей для распределенного Cache (Redis)
    /// </summary>
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
