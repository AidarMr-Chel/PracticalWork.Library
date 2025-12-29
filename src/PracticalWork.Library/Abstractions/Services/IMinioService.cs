using Microsoft.AspNetCore.Http;

namespace PracticalWork.Library.Abstractions.Services
{
    /// <summary>
    /// Сервис для работы с объектным хранилищем MinIO.
    /// Предоставляет базовые операции загрузки, удаления и получения ссылок на файлы.
    /// </summary>
    public interface IMinioService
    {
        /// <summary>
        /// Загружает файл в хранилище MinIO.
        /// </summary>
        /// <param name="stream">Поток данных загружаемого файла.</param>
        /// <param name="objectName">Имя объекта (ключ), под которым файл будет сохранён.</param>
        /// <param name="contentType">MIME‑тип файла.</param>
        /// <returns>Путь или URL загруженного объекта.</returns>
        Task<string> UploadAsync(Stream stream, string objectName, string contentType);

        /// <summary>
        /// Удаляет файл из хранилища MinIO.
        /// </summary>
        /// <param name="objectName">Имя объекта (ключ), который необходимо удалить.</param>
        Task DeleteAsync(string objectName);

        /// <summary>
        /// Возвращает публичный URL файла в хранилище MinIO.
        /// Может зависеть от настроек доступа к bucket.
        /// </summary>
        /// <param name="objectName">Имя объекта.</param>
        /// <returns>URL файла.</returns>
        Task<string> GetFileUrlAsync(string objectName);
    }
}
