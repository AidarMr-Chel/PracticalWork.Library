using Microsoft.Extensions.Logging;
using PracticalWork.Library.Services;
using Quartz;

/// <summary>
/// Quartz‑задача, запускающая воркфлоу отправки напоминаний о возврате книг.
/// Выполняет только запуск по расписанию и логирование результата.
/// </summary>
public sealed class ReturnReminderJob : IJob
{
    private readonly ReturnReminderWorkflow _workflow;
    private readonly ILogger<ReturnReminderJob> _logger;

    /// <summary>
    /// Создаёт экземпляр задачи отправки напоминаний.
    /// </summary>
    /// <param name="workflow">Воркфлоу, выполняющий бизнес‑логику отправки напоминаний.</param>
    /// <param name="logger">Логгер для записи информации о выполнении задачи.</param>
    public ReturnReminderJob(
        ReturnReminderWorkflow workflow,
        ILogger<ReturnReminderJob> logger)
    {
        _workflow = workflow;
        _logger = logger;
    }

    /// <summary>
    /// Выполняет задачу отправки напоминаний читателям.
    /// </summary>
    /// <param name="context">Контекст выполнения Quartz‑задачи.</param>
    public Task Execute(IJobExecutionContext context)
        => _workflow.ExecuteAsync(context.CancellationToken);
}
