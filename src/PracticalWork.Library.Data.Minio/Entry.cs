using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Services;

namespace PracticalWork.Library.Data.Minio
{
    public static class Entry
    {
        /// <summary>
        /// Регистрирует в контейнере сервисов файловое хранилище MinIO.
        /// Настройки подключения (endpoint, ключи, bucket) берутся из IConfiguration.
        /// </summary>
        /// <param name="services">Коллекция сервисов приложения.</param>
        /// <param name="configuration">Конфигурация приложения, содержащая параметры MinIO.</param>
        /// <returns>
        /// Коллекция сервисов с добавленной поддержкой MinIO‑хранилища.
        /// </returns>
        public static IServiceCollection AddMinioFileStorage(this IServiceCollection services, IConfiguration configuration)
        {
            var endpoint = configuration["App:Minio:Endpoint"];
            var accessKey = configuration["App:Minio:AccessKey"];
            var secretKey = configuration["App:Minio:SecretKey"];
            var bucket = configuration["App:Minio:Bucket"];

            services.AddSingleton<IMinioClient>(sp =>
            {
                var endpoint = configuration["App:Minio:Endpoint"];
                var accessKey = configuration["App:Minio:AccessKey"];
                var secretKey = configuration["App:Minio:SecretKey"];

                return new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                    .Build();
            });

            services.AddScoped<IMinioService>(sp =>
            {
                var client = sp.GetRequiredService<IMinioClient>();
                var bucket = configuration["App:Minio:Bucket"];
                return new MinioService(client, bucket);
            });

            return services;
        }
    }
}
