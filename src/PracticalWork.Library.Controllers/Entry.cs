using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Library.Controllers.Validations.v1;
using System.Globalization;

namespace PracticalWork.Library.Controllers
{
    /// <summary>
    /// Точка входа для конфигурации и подключения API приложения.
    /// Содержит методы расширения для регистрации валидации,
    /// версионирования и контроллеров.
    /// </summary>
    public static class Entry
    {
        /// <summary>
        /// Добавляет API‑функциональность в приложение.
        /// Регистрирует валидацию, версионирование и контроллеры.
        /// </summary>
        /// <param name="builder">MVC‑конфигуратор.</param>
        /// <returns>Тот же <see cref="IMvcBuilder"/> для цепочки вызовов.</returns>
        public static IMvcBuilder AddApi(this IMvcBuilder builder)
        {
            builder.Services.AddValidation();
            builder.Services.AddApiVersioning();
            builder.AddApplicationPart(typeof(Api.v1.BooksController).Assembly);

            return builder;
        }

        /// <summary>
        /// Регистрирует FluentValidation и глобальные настройки валидации.
        /// </summary>
        /// <param name="services">Коллекция сервисов DI.</param>
        private static void AddValidation(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CreateBookRequestValidator>();
            services.AddFluentValidationAutoValidation();

            ValidatorOptions.Global.DisplayNameResolver = (_, member, _) => member?.Name;
            ValidatorOptions.Global.LanguageManager.Culture = CultureInfo.GetCultureInfo("ru");
        }

        /// <summary>
        /// Добавляет поддержку версионирования API и конфигурацию API Explorer.
        /// </summary>
        /// <param name="services">Коллекция сервисов DI.</param>
        private static void AddApiVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1.0);
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
                .AddApiExplorer(options =>
                {
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });
        }
    }
}
