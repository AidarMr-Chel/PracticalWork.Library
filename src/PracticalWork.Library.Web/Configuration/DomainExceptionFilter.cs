using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PracticalWork.Library.Web.Configuration;

/// <summary>
/// Фильтр предназначен для трансформации доменных исключений в Bad Request
/// </summary>
/// <typeparam name="TAppException"> Доменное исключение </typeparam>
[UsedImplicitly]
public class DomainExceptionFilter<TAppException> : IAsyncActionFilter where TAppException : Exception
{
    protected readonly ILogger Logger;

    public DomainExceptionFilter(ILogger<DomainExceptionFilter<TAppException>> logger)
    {
        Logger = logger;
    }
    /// <summary>
    /// Обработка исключений после выполнения действия контроллера
    /// </summary>
    /// <param name="context"></param>
    /// <param name="next"></param>
    /// <returns></returns>
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();
        if (HasException(resultContext))
        {
            TryHandleException(resultContext, resultContext.Exception);
        }
    }

    /// <summary>
    /// Проверка наличия необработанного исключения в контексте
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static bool HasException(ActionExecutedContext context) => context.Exception != null && !context.ExceptionHandled;

    /// <summary>
    /// Попытка обработать исключение определенного типа
    /// </summary>
    /// <param name="context"></param>
    /// <param name="exception"></param>
    protected virtual void TryHandleException(ActionExecutedContext context, Exception exception)
    {
        if (exception is not TAppException)
            return;

        var problemDetails = BuildProblemDetails(exception);

        context.Result = new BadRequestObjectResult(problemDetails);
        context.ExceptionHandled = true;

        Logger.LogError(exception, "Unhandled domain exception. Transformed to Bad request (400).");
    }

    /// <summary>
    /// Построение объекта ValidationProblemDetails на основе исключения
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    protected static ValidationProblemDetails BuildProblemDetails(Exception exception)
    {
        var exceptionName = exception.GetType().Name;
        var errorMessages = new[] { exception.Message };

        // Используем ValidationProblemDetails, а не базовый или свой тип, т. к. он обвешан атрибутами сериализации
        // и так мы можем гарантировать идентичный ответ и при ошибках валидации, и при доменных исключениях:
        var problemDetails = new ValidationProblemDetails
        {
            Title = "Произошла ошибка во время выполнения запроса.",
            Errors = { { exceptionName, errorMessages } }
        };

        return problemDetails;
    }
}
