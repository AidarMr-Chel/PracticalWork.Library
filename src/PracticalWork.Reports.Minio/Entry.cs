using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace PracticalWork.Reports.Minio;

/// <summary>
/// Точка входа модуля MinIO.
/// Содержит методы для регистрации клиента MinIO и сервисов,
/// обеспечивающих работу с объектным хранилищем.
/// </summary>
public static class Entry
{
    /// <summary>
    /// Регистрирует зависимости, необходимые для работы с MinIO.
    /// Настройки подключения берутся из конфигурации приложения.
    /// </summary>
    /// <param name="services">Коллекция сервисов приложения.</param>
    /// <param name="config">Конфигурация приложения, содержащая параметры MinIO.</param>
    /// <returns>Коллекция сервисов с добавленными зависимостями.</returns>
    /// <exception cref="InvalidOperationException">
    /// Выбрасывается, если не указан обязательный параметр Bucket.
    /// </exception>
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
