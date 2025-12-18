namespace PracticalWork.Reports.Minio;

public interface IMinioService
{
    Task<string> UploadAsync(Stream stream, string objectName, string contentType);
    Task DeleteAsync(string objectName);
    Task<string> GetFileUrlAsync(string objectName);
    Task<List<MinioObjectInfo>> ListAsync(string prefix);
    Task<bool> ExistsAsync(string objectName);
    Task<string> GetSignedUrlAsync(string objectName, TimeSpan expiry);

}
