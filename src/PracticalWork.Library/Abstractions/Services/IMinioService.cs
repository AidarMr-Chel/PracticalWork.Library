using Microsoft.AspNetCore.Http;

namespace PracticalWork.Library.Abstractions.Services
{
    public interface IMinioService
    {
        Task<string> UploadAsync(Stream stream, string objectName, string contentType);
        Task DeleteAsync(string objectName);
        Task<string> GetFileUrlAsync(string objectName);
    }
}
