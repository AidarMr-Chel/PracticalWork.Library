namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Сервис для загрузки и рендеринга HTML/TXT‑шаблонов писем.
/// Позволяет подставлять параметры в шаблон по ключам.
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Рендерит шаблон письма, заменяя плейсхолдеры вида {Key}
    /// на соответствующие значения из словаря параметров.
    /// </summary>
    /// <param name="templateFileName">Имя файла шаблона (HTML или TXT).</param>
    /// <param name="parameters">Словарь параметров, где ключ соответствует плейсхолдеру в шаблоне.</param>
    /// <returns>Готовый текст письма с подставленными значениями.</returns>
    Task<string> RenderAsync(string templateFileName, IDictionary<string, string> parameters);
}
