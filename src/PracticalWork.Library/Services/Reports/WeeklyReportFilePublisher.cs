using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Models;

namespace PracticalWork.Library.Services.Reports;

/// <summary>
/// Загружает CSV-отчёт в MinIO и возвращает presigned URL.
/// </summary>
public sealed class WeeklyReportFilePublisher : IWeeklyReportFilePublisher
{
    private readonly IMinioService _minioService;
    private readonly MinioReportsSettings _settings;
    private readonly ILogger<WeeklyReportFilePublisher> _logger;

    public WeeklyReportFilePublisher(
        IMinioService minioService,
        IOptions<MinioReportsSettings> settings,
        ILogger<WeeklyReportFilePublisher> logger)
    {
        _minioService = minioService;
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> PublishAsync(byte[] csvBytes, string fileName, CancellationToken cancellationToken = default)
    {
        using var csvStream = new MemoryStream(csvBytes);

        await _minioService.UploadToBucketAsync(
            csvStream,
            fileName,
            "text/csv; charset=utf-8",
            _settings.Bucket);

        _logger.LogInformation(
            "Файл загружен в бакет '{Bucket}': {FileName}",
            _settings.Bucket,
            fileName);

        var expiry = TimeSpan.FromSeconds(_settings.PresignedUrlExpirySeconds);
        var fileUrl = await _minioService.GetPresignedUrlAsync(fileName, expiry, _settings.Bucket);

        _logger.LogInformation("Presigned‑ссылка (действует {Expiry}): {Url}", expiry, fileUrl);

        return fileUrl;
    }
}
