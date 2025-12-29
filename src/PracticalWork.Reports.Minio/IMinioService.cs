namespace PracticalWork.Reports.Minio;

/// <summary>
/// Интерфейс для работы с MinIO
/// </summary>
public interface IMinioService
{
    /// <summary>
    /// Загрузка файла в хранилище MinIO
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="objectName"></param>
    /// <param name="contentType"></param>
    /// <returns></returns>
    Task<string> UploadAsync(Stream stream, string objectName, string contentType);
    /// <summary>
    /// Удаление файла из хранилища MinIO
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    Task DeleteAsync(string objectName);
    /// <summary>
    /// Получение URL файла в хранилище MinIO
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    Task<string> GetFileUrlAsync(string objectName);
    /// <summary>
    /// Получение списка файлов в хранилище MinIO по префиксу
    /// </summary>
    /// <param name="prefix"></param>
    /// <returns></returns>
    Task<List<MinioObjectInfo>> ListAsync(string prefix);
    /// <summary>
    /// Проверка существования файла в хранилище MinIO
    /// </summary>
    /// <param name="objectName"></param>
    /// <returns></returns>
    Task<bool> ExistsAsync(string objectName);
    /// <summary>
    /// Получение подписанного URL файла в хранилище MinIO
    /// </summary>
    /// <param name="objectName"></param>
    /// <param name="expiry"></param>
    /// <returns></returns>
    Task<string> GetSignedUrlAsync(string objectName, TimeSpan expiry);

}
