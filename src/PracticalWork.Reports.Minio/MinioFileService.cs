using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace PracticalWork.Reports.Minio;

/// <summary>
/// Сервис для работы с объектным хранилищем MinIO.
/// Предоставляет операции загрузки, удаления, получения ссылок,
/// проверки существования и перечисления объектов.
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
    /// Загружает файл в хранилище MinIO.
    /// При необходимости автоматически создаёт bucket.
    /// </summary>
    /// <param name="stream">Поток данных загружаемого файла.</param>
    /// <param name="objectName">Имя объекта (ключ), под которым файл будет сохранён.</param>
    /// <param name="contentType">MIME‑тип содержимого.</param>
    /// <returns>Путь к объекту в формате <c>bucket/objectName</c>.</returns>
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
    /// Удаляет объект из хранилища MinIO.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    public async Task DeleteAsync(string objectName)
    {
        var args = new RemoveObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectName);

        await _client.RemoveObjectAsync(args);
    }

    /// <summary>
    /// Возвращает публичный URL объекта.
    /// Подходит только для bucket'ов с публичным доступом.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <returns>URL файла.</returns>
    public Task<string> GetFileUrlAsync(string objectName)
    {
        return Task.FromResult($"http://localhost:9000/{_bucket}/{objectName}");
    }

    /// <summary>
    /// Проверяет существование bucket и создаёт его при отсутствии.
    /// </summary>
    private async Task EnsureBucketExistsAsync()
    {
        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucket));
        if (!exists)
        {
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucket));
        }
    }

    /// <summary>
    /// Возвращает список объектов, имена которых начинаются с указанного префикса.
    /// </summary>
    /// <param name="prefix">Префикс имени объекта.</param>
    /// <returns>Список объектов MinIO.</returns>
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
    /// Проверяет существование объекта в хранилище MinIO.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <returns><c>true</c>, если объект существует; иначе <c>false</c>.</returns>
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
    /// Возвращает подписанный (временный) URL для приватного объекта.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <param name="expiry">Время жизни ссылки.</param>
    /// <returns>Подписанный URL.</returns>
    public async Task<string> GetSignedUrlAsync(string objectName, TimeSpan expiry)
    {
        var args = new PresignedGetObjectArgs()
            .WithBucket(_bucket)
            .WithObject(objectName)
            .WithExpiry((int)expiry.TotalSeconds);

        return await _client.PresignedGetObjectAsync(args);
    }
}
