/// <summary>
/// Информация об объекте в MinIO
/// </summary>
public class MinioObjectInfo
{
    /// <summary>
    /// Ключ объекта
    /// </summary>
    public string Key { get; set; } = default!;
    /// <summary>
    /// Размер объекта в байтах
    /// </summary>
    public DateTime? LastModifiedDate { get; set; }
}
