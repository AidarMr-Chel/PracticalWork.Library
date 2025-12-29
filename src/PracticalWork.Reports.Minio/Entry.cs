using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace PracticalWork.Reports.Minio;

/// <summary>
/// Точка входа модуля MinIO
/// </summary>
public static class Entry
{
    /// <summary>
    /// Добавление зависимостей для работы с MinIO
    /// </summary>
    /// <param name="services"></param>
    /// <param name="config"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IServiceCollection AddMinioModule(this IServiceCollection services, IConfiguration config)
    {
        var endpoint = config["App:Minio:Endpoint"];
        var accessKey = config["App:Minio:AccessKey"];
        var secretKey = config["App:Minio:SecretKey"];
        var bucket = config["App:Minio:Bucket"]
            ?? throw new InvalidOperationException("MinIO bucket is not configured");

        services.AddSingleton<IMinioClient>(_ =>
            new MinioClient()
                .WithEndpoint(endpoint)
                .WithCredentials(accessKey, secretKey)
                .Build());

        services.AddScoped<IMinioService>(_ =>
        {
            var client = _.GetRequiredService<IMinioClient>();
            return new MinioService(client, bucket);
        });

        return services;
    }
}
