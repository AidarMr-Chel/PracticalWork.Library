using Minio;
using Minio.DataModel.Args;
using PracticalWork.Library.Abstractions.Services;

public sealed class MinioService : IMinioService
{
    private readonly IMinioClient _client;
    private readonly string _bucketName;

    public MinioService(IMinioClient client, string bucketName)
    {
        _client = client;
        _bucketName = bucketName;
    }

    /// <summary>
    /// Загружает файл в MinIO‑хранилище.
    /// Перед загрузкой гарантирует существование бакета.
    /// </summary>
    /// <param name="stream">Поток данных загружаемого файла.</param>
    /// <param name="objectName">Имя объекта (файла) в хранилище.</param>
    /// <param name="contentType">MIME‑тип загружаемого файла.</param>
    /// <returns>
    /// Путь к загруженному объекту в формате bucket/objectName.
    /// </returns>
    public async Task<string> UploadAsync(Stream stream, string objectName, string contentType)
    {
        await EnsureBucketExistsAsync();

        await _client.PutObjectAsync(new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName)
            .WithStreamData(stream)
            .WithObjectSize(stream.Length)
            .WithContentType(contentType));

        return $"{_bucketName}/{objectName}";
    }

    /// <summary>
    /// Удаляет файл из MinIO‑хранилища.
    /// </summary>
    /// <param name="objectName">Имя удаляемого объекта.</param>
    public async Task DeleteAsync(string objectName)
    {
        await _client.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName));
    }

    /// <summary>
    /// Возвращает URL файла в MinIO‑хранилище.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <returns>
    /// Полный URL файла.  
    /// </returns>
    public Task<string> GetFileUrlAsync(string objectName)
    {
        return Task.FromResult($"http://localhost:9000/{_bucketName}/{objectName}");
    }

    /// <summary>
    /// Проверяет существование бакета и создаёт его при необходимости.
    /// </summary>
    private async Task EnsureBucketExistsAsync()
    {
        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!exists)
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
    }
}
