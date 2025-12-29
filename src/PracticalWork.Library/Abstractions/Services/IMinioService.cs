using Microsoft.AspNetCore.Http;

namespace PracticalWork.Library.Abstractions.Services
{
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
    }
}
