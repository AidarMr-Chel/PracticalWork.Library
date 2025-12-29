namespace PracticalWork.Reports.Minio;

/// <summary>
/// Интерфейс сервиса для работы с объектным хранилищем MinIO.
/// Предоставляет операции загрузки, удаления, получения ссылок и проверки существования объектов.
/// </summary>
public interface IMinioService
{
    /// <summary>
    /// Загружает файл в хранилище MinIO.
    /// </summary>
    /// <param name="stream">Поток данных загружаемого файла.</param>
    /// <param name="objectName">Имя объекта (ключ), под которым файл будет сохранён.</param>
    /// <param name="contentType">MIME‑тип содержимого.</param>
    /// <returns>Путь или URL загруженного объекта.</returns>
    Task<string> UploadAsync(Stream stream, string objectName, string contentType);

    /// <summary>
    /// Удаляет файл из хранилища MinIO.
    /// </summary>
    /// <param name="objectName">Имя объекта (ключ), который необходимо удалить.</param>
    Task DeleteAsync(string objectName);

    /// <summary>
    /// Получает публичный URL файла в хранилище MinIO.
    /// Может зависеть от настроек доступа к bucket.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <returns>URL файла.</returns>
    Task<string> GetFileUrlAsync(string objectName);

    /// <summary>
    /// Возвращает список объектов в хранилище MinIO,
    /// имена которых начинаются с указанного префикса.
    /// </summary>
    /// <param name="prefix">Префикс имени объекта.</param>
    /// <returns>Список объектов MinIO.</returns>
    Task<List<MinioObjectInfo>> ListAsync(string prefix);

    /// <summary>
    /// Проверяет существование файла в хранилище MinIO.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <returns><c>true</c>, если объект существует; иначе <c>false</c>.</returns>
    Task<bool> ExistsAsync(string objectName);

    /// <summary>
    /// Получает подписанный (временный) URL для доступа к объекту.
    /// Используется для приватных файлов.
    /// </summary>
    /// <param name="objectName">Имя объекта.</param>
    /// <param name="expiry">Время жизни ссылки.</param>
    /// <returns>Подписанный URL.</returns>
    Task<string> GetSignedUrlAsync(string objectName, TimeSpan expiry);
}
