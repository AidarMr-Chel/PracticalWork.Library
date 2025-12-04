using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

        return services;
    }
}
