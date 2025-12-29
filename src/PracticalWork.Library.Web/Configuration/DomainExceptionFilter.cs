using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PracticalWork.Library.Web.Configuration
{
    /// <summary>
    /// Фильтр для преобразования доменных исключений в ответ формата BadRequest (400).
    /// Позволяет унифицировать обработку ошибок доменной логики.
    /// </summary>
    /// <typeparam name="TAppException">Тип доменного исключения.</typeparam>
    [UsedImplicitly]
    public class DomainExceptionFilter<TAppException> : IAsyncActionFilter
        where TAppException : Exception
    {
        /// <summary>
        /// Логгер для записи информации об ошибках.
        /// </summary>
        protected readonly ILogger Logger;

        /// <summary>
        /// Создаёт новый экземпляр фильтра обработки доменных исключений.
        /// </summary>
        /// <param name="logger">Логгер для записи ошибок.</param>
        public DomainExceptionFilter(ILogger<DomainExceptionFilter<TAppException>> logger)
        {
            Logger = logger;
        }

        /// <summary>
        /// Выполняет обработку исключений после выполнения действия контроллера.
        /// </summary>
        /// <param name="context">Контекст выполнения действия.</param>
        /// <param name="next">Делегат выполнения следующего этапа конвейера.</param>
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (HasException(resultContext))
            {
                TryHandleException(resultContext, resultContext.Exception!);
            }
        }

        /// <summary>
        /// Проверяет, содержит ли контекст необработанное исключение.
        /// </summary>
        /// <param name="context">Контекст выполнения действия.</param>
        private static bool HasException(ActionExecutedContext context) =>
            context.Exception != null && !context.ExceptionHandled;

        /// <summary>
        /// Пытается обработать исключение указанного типа и преобразовать его в BadRequest.
        /// </summary>
        /// <param name="context">Контекст выполнения действия.</param>
        /// <param name="exception">Исключение, возникшее при выполнении действия.</param>
        protected virtual void TryHandleException(ActionExecutedContext context, Exception exception)
        {
            if (exception is not TAppException)
                return;

            var problemDetails = BuildProblemDetails(exception);

            context.Result = new BadRequestObjectResult(problemDetails);
            context.ExceptionHandled = true;

            Logger.LogError(exception, "Unhandled domain exception. Transformed to BadRequest (400).");
        }

        /// <summary>
        /// Создаёт объект <see cref="ValidationProblemDetails"/> на основе исключения.
        /// Используется единый формат ошибок для валидации и доменных исключений.
        /// </summary>
        /// <param name="exception">Исключение, содержащее сообщение об ошибке.</param>
        /// <returns>Объект <see cref="ValidationProblemDetails"/>.</returns>
        protected static ValidationProblemDetails BuildProblemDetails(Exception exception)
        {
            var exceptionName = exception.GetType().Name;
            var errorMessages = new[] { exception.Message };

            var problemDetails = new ValidationProblemDetails
            {
                Title = "Произошла ошибка во время выполнения запроса.",
                Errors = { { exceptionName, errorMessages } }
            };

            return problemDetails;
        }
    }
}
