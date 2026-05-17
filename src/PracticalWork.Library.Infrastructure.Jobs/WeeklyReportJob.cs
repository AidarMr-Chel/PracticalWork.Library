using Microsoft.Extensions.Logging;
using PracticalWork.Library.Services;
using Quartz;

/// <summary>
/// Quartz‑задача, запускающая воркфлоу формирования еженедельного отчёта.
/// Отвечает за запуск по расписанию и логирование результата.
/// </summary>
public sealed class WeeklyReportJob : IJob
{
    private readonly WeeklyReportWorkflow _workflow;
    private readonly ILogger<WeeklyReportJob> _logger;

    /// <summary>
    /// Создаёт экземпляр задачи формирования еженедельного отчёта.
    /// </summary>
    /// <param name="workflow">Воркфлоу, выполняющий бизнес‑логику формирования отчёта.</param>
    /// <param name="logger">Логгер для записи информации о выполнении задачи.</param>
    public WeeklyReportJob(
        WeeklyReportWorkflow workflow,
        ILogger<WeeklyReportJob> logger)
    {
        _workflow = workflow;
        _logger = logger;
    }

    /// <summary>
    /// Выполняет задачу формирования и отправки еженедельного отчёта.
    /// </summary>
    /// <param name="context">Контекст выполнения Quartz‑задачи.</param>
    public Task Execute(IJobExecutionContext context)
        => _workflow.ExecuteAsync(context.CancellationToken);
}
