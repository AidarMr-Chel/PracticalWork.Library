namespace PracticalWork.Library.Models;

/// <summary>
/// Настройки MinIO для модуля еженедельных отчетов.
/// </summary>
public class MinioReportsSettings
{
    /// <summary>
    /// Имя бакета для хранения файлов отчетов.
    /// По умолчанию: "reports".
    /// </summary>
    public string Bucket { get; set; } = "reports";

    /// <summary>
    /// Срок хранения файлов в бакете (в днях).
    /// Используется для настройки Lifecycle policy в MinIO.
    /// По умолчанию: 90 дней.
    /// </summary>
    public int RetentionDays { get; set; } = 90;

    /// <summary>
    /// Срок действия presigned-ссылки в секундах.
    /// По умолчанию: 7 дней (604800 секунд).
    /// </summary>
    public int PresignedUrlExpirySeconds { get; set; } = 604800;
}