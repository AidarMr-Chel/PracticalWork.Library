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
    /// Загружает файл в хранилище
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="objectName"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
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
    /// Удаляет файл из хранилища
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public async Task DeleteAsync(string objectName)
    {
        await _client.RemoveObjectAsync(new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(objectName));
    }

    /// <summary>
    /// Возвращает URL файла в хранилище
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    public Task<string> GetFileUrlAsync(string objectName)
    {
        return Task.FromResult($"http://localhost:9000/{_bucketName}/{objectName}");
    }

    /// <summary>
    /// Возвращает true, если указанный бакет существует, иначе создает его.
    /// </summary>
    /// <returns></returns>
    private async Task EnsureBucketExistsAsync()
    {
        var exists = await _client.BucketExistsAsync(new BucketExistsArgs().WithBucket(_bucketName));
        if (!exists)
            await _client.MakeBucketAsync(new MakeBucketArgs().WithBucket(_bucketName));
    }
}
