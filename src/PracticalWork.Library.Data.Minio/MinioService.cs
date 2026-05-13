using Minio;
using Minio.DataModel.Args;
using PracticalWork.Library.Abstractions.Services;

namespace PracticalWork.Library.Data.Minio;

/// <summary>
/// Реализация сервиса для работы с объектным хранилищем MinIO.
/// </summary>
public sealed class MinioService : IMinioService
{
    private readonly IMinioClient _client;
    private readonly string _defaultBucket;

    public MinioService(IMinioClient client, string defaultBucket)
    {
        _client = client;
        _defaultBucket = defaultBucket;
    }

    /// <inheritdoc />
    public async Task<string> UploadAsync(Stream stream, string objectName, string contentType)
    {
        await EnsureBucketExistsAsync(_defaultBucket);
        return await UploadInternalAsync(stream, objectName, contentType, _defaultBucket);
    }

    /// <inheritdoc />
    public async Task<string> UploadToBucketAsync(Stream stream, string objectName, string contentType, string bucketName)
    {
        await EnsureBucketExistsAsync(bucketName);
        return await UploadInternalAsync(stream, objectName, contentType, bucketName);
    }

    /// <summary>
    /// Внутренний метод загрузки для избежания дублирования кода.
    /// </summary>
    private async Task<string> UploadInternalAsync(Stream stream, string objectName, string contentType, string bucketName)
    {
        // Сбрасываем позицию потока, если он уже читался
        if (stream.Position > 0)
            stream.Position = 0;

        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType));

        return $"{bucketName}/{objectName}";
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string objectName)
    {
        await _client.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_defaultBucket)
            .WithObject(objectName));
    }

    /// <inheritdoc />
    public Task<string> GetFileUrlAsync(string objectName)
        => Task.FromResult($"http://localhost:9000/{_defaultBucket}/{objectName}");

    /// <inheritdoc />
    public Task<string> GetFileUrlAsync(string objectName, string bucketName)
        => Task.FromResult($"http://localhost:9000/{bucketName}/{objectName}");

    /// <inheritdoc />
    public async Task<string> GetPresignedUrlAsync(string objectName, TimeSpan expiry, string bucketName = null)
    {
        var targetBucket = bucketName ?? _defaultBucket;

        return await _client.PresignedGetObjectAsync(new PresignedGetObjectArgs()
            .WithBucket(targetBucket)
            .WithObject(objectName)
            .WithExpiry((int)expiry.TotalSeconds));
    }

    /// <summary>
    /// Проверяет существование бакета и создаёт его при необходимости.
    /// </summary>
    private async Task EnsureBucketExistsAsync(string bucketName)
    {
        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));
        if (!exists)
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
        }
    }
}