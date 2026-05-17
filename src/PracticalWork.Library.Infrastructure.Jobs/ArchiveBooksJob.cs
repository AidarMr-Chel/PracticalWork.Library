using Microsoft.Extensions.Logging;
using PracticalWork.Library.Services;
using Quartz;

/// <summary>
/// Quartz‑задача, запускающая воркфлоу архивации книг.
/// Отвечает только за вызов бизнес‑логики и логирование результата.
/// </summary>
public sealed class ArchiveBooksJob : IJob
{
    private readonly ArchiveBooksWorkflow _workflow;
    private readonly ILogger<ArchiveBooksJob> _logger;

    /// <summary>
    /// Создаёт экземпляр задачи архивации книг.
    /// </summary>
    /// <param name="workflow">Воркфлоу, выполняющий бизнес‑логику архивации.</param>
    /// <param name="logger">Логгер для записи информации о выполнении задачи.</param>
    public ArchiveBooksJob(
        ArchiveBooksWorkflow workflow,
        ILogger<ArchiveBooksJob> logger)
    {
        _workflow = workflow;
        _logger = logger;
    }

    /// <summary>
    /// Выполняет задачу архивации книг.
    /// </summary>
    /// <param name="context">Контекст выполнения Quartz‑задачи.</param>
    public Task Execute(IJobExecutionContext context)
        => _workflow.ExecuteAsync(context.CancellationToken);
}
