using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace PracticalWork.Reports.Minio;

/// <summary>
/// Сервис для работы с MinIO
/// </summary>
public class MinioService : IMinioService
{
    private readonly IMinioClient _client;
    private readonly string _bucket;

    public MinioService(IMinioClient client, string bucket)
    {
        _client = client;
        _bucket = bucket;
    }

    /// <summary>
    /// Загрузка файла в хранилище MinIO
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="objectName"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    public async Task<string> UploadAsync(Stream stream, string objectName, string contentType)
    {
        await EnsureBucketExistsAsync();

        var args = new PutObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType);

        await _client.PutObjectAsync(args);

        return $"{_bucket}/{objectName}";
    }

    /// <summary>
    /// Удаление файла из хранилища MinIO
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public async Task DeleteAsync(string objectName)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectName);

        await _client.RemoveObjectAsync(args);
    }

    /// <summary>
    /// Получение URL файла в хранилище MinIO
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public Task<string> GetFileUrlAsync(string objectName)
    {
        return Task.FromResult($"http://localhost:9000/{_bucket}/{objectName}");
    }

    /// <summary>
    /// Проверка существования корзины, создание при отсутствии
    /// </summary>
    /// <returns></returns>
    private async Task EnsureBucketExistsAsync()
    {
        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
        if (!exists)
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));
        }
    }

    /// <summary>
    /// Получение списка файлов в хранилище MinIO по префиксу
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    public async Task<List<MinioObjectInfo>> ListAsync(string prefix)
    {
        var result = new List<MinioObjectInfo>();

        var args = new ListObjectsArgs()
            .WithBucket(_bucket)
            .WithPrefix(prefix)
            .WithRecursive(true);

        IAsyncEnumerable<Item> objects = _client.ListObjectsEnumAsync(args);

        await foreach (var item in objects)
        {
            result.Add(new MinioObjectInfo
            {
                Key = item.Key,
                LastModifiedDate = DateTime.TryParse(item.LastModified, out var dt)
                    ? dt
                    : (DateTime?)null
            });
        }

        return result;
    }

    /// <summary>
    /// Проверка существования файла в хранилище MinIO
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public async Task<bool> ExistsAsync(string objectName)
    {
        try
        {
            var args = new StatObjectArgs()
                .WithBucket(_bucket)
                .WithObject(objectName);

            await _client.StatObjectAsync(args);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Получение подписанного URL файла в хранилище MinIO
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    public async Task<string> GetSignedUrlAsync(string objectName, TimeSpan expiry)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectName)
            .WithExpiry((int)expiry.TotalSeconds);

        return await _client.PresignedGetObjectAsync(args);
    }


}
