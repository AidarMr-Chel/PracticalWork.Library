using System.Collections.Concurrent;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PracticalWork.Library.Abstractions.Services;

namespace PracticalWork.Library.Services;

/// <summary>
/// Сервис загрузки и рендеринга HTML‑шаблонов писем из файловой системы.
/// Использует кэширование для повышения производительности.
/// </summary>
public sealed class FileEmailTemplateService : IEmailTemplateService
{
    private readonly string _templatesPath;
    private readonly ConcurrentDictionary<string, string> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<FileEmailTemplateService> _logger;

    /// <summary>
    /// Создаёт экземпляр сервиса рендеринга email‑шаблонов.
    /// </summary>
    /// <param name="env">Хост‑окружение приложения.</param>
    /// <param name="logger">Логгер.</param>
    public FileEmailTemplateService(IHostEnvironment env, ILogger<FileEmailTemplateService> logger)
    {
        _logger = logger;

        _templatesPath = Path.Combine(env.ContentRootPath, "Templates", "Emails");

        if (!Directory.Exists(_templatesPath))
        {
            _logger.LogWarning("Папка с шаблонами email не найдена: {Path}", _templatesPath);
            return;
        }

        var files = Directory.GetFiles(_templatesPath, "*.html");
        _logger.LogInformation("Email‑шаблоны: найдено {Count} файлов в {Path}", files.Length, _templatesPath);
    }

    /// <summary>
    /// Рендерит шаблон письма, подставляя параметры вида {Key}.
    /// </summary>
    /// <param name="templateFileName">Имя файла шаблона.</param>
    /// <param name="parameters">Словарь параметров для подстановки.</param>
    /// <returns>Готовый HTML‑текст письма.</returns>
    public Task<string> RenderAsync(string templateFileName, IDictionary<string, string> parameters)
    {
        var template = _cache.GetOrAdd(templateFileName, fileName =>
        {
            var filePath = Path.Combine(_templatesPath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Email template not found: {filePath}");

            _logger.LogDebug("Загружен шаблон email: {File}", fileName);
            return File.ReadAllText(filePath);
        });

        var rendered = template;

        foreach (var kvp in parameters)
        {
            rendered = rendered.Replace($"{{{kvp.Key}}}", kvp.Value ?? string.Empty);
        }

        return Task.FromResult(rendered);
    }
}
