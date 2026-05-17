namespace PracticalWork.Library.Abstractions.Services;

/// <summary>
/// Базовый интерфейс для бизнес‑воркфлоу, запускаемых по расписанию или вручную.
/// Определяет единый контракт выполнения бизнес‑процесса.
/// </summary>
public interface IWorkflow
{
    /// <summary>
    /// Выполняет бизнес‑процесс.
    /// Возвращает результат выполнения, содержащий информацию об успехе, количестве обработанных элементов и ошибках.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат выполнения воркфлоу.</returns>
    Task<WorkflowResult> ExecuteAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Результат выполнения воркфлоу.
/// Используется Quartz‑задачами и сервисами для логирования и анализа.
/// </summary>
/// <param name="IsSuccess">Признак успешного выполнения.</param>
/// <param name="ProcessedCount">Количество успешно обработанных элементов.</param>
/// <param name="ErrorCount">Количество ошибок.</param>
/// <param name="ErrorMessage">Описание ошибки, если выполнение завершилось неуспешно.</param>
public sealed record WorkflowResult(
    bool IsSuccess,
    int ProcessedCount,
    int ErrorCount,
    string ErrorMessage = null);
