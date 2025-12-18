using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace PracticalWork.Reports.Minio;

public static class Entry
{
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
