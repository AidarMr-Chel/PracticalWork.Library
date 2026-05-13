using Microsoft.AspNetCore.Http;

namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис для работы с объектным хранилищем MinIO.
/// Предоставляет базовые операции загрузки, удаления и получения ссылок на файлы.
/// </summary>
public interface IMinioService
{
    /// <summary>
    /// Загружает файл в хранилище MinIO (в дефолтный бакет).
    /// </summary>
    Task<string> UploadAsync(Stream stream, string objectName, string contentType);

    /// <summary>
    /// Загружает файл в указанный бакет.
    /// </summary>
    Task<string> UploadToBucketAsync(Stream stream, string objectName, string contentType, string bucketName);

    /// <summary>
    /// Удаляет файл из хранилища MinIO.
    /// </summary>
    Task DeleteAsync(string objectName);

    /// <summary>
    /// Возвращает публичный URL файла в дефолтном бакете.
    /// Работает только для публичных бакетов.
    /// </summary>
    Task<string> GetFileUrlAsync(string objectName);

    /// <summary>
    /// Возвращает публичный URL файла в указанном бакете.
    /// Работает только для публичных бакетов.
    /// </summary>
    Task<string> GetFileUrlAsync(string objectName, string bucketName);

    /// <summary>
    /// Возвращает временную подписанную ссылку (presigned URL) для приватного объекта.
    /// Ссылка действительна в течение указанного времени.
    /// </summary>
    /// <param name="objectName">Имя объекта в хранилище.</param>
    /// <param name="expiry">Срок действия ссылки.</param>
    /// <param name="bucketName">Имя бакета (опционально, по умолчанию — дефолтный).</param>
    /// <returns>Presigned URL для скачивания файла.</returns>
    Task<string> GetPresignedUrlAsync(string objectName, TimeSpan expiry, string bucketName = null);
}