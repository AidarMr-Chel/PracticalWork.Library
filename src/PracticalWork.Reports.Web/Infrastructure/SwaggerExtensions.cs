using System.Reflection;
using Microsoft.OpenApi.Models;

namespace PracticalWork.Reports.Web.Infrastructure;

/// <summary>
/// Настройка Swagger/OpenAPI для Reports API.
/// </summary>
public static class SwaggerExtensions
{
    public static IServiceCollection AddReportsSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "PracticalWork.Reports API",
                Version = "v1",
                Description =
                    "Сервис отчётов и логов активности библиотеки. " +
                    "Принимает события из RabbitMQ, хранит логи в PostgreSQL, " +
                    "генерирует CSV-отчёты в MinIO.",
                Contact = new OpenApiContact
                {
                    Name = "PracticalWork.Library",
                },
            });

            IncludeAllXmlComments(options);

            options.TagActionsBy(api =>
                new[] { api.ActionDescriptor.RouteValues["controller"] ?? "API" });
            options.DocInclusionPredicate((_, _) => true);
        });

        return services;
    }

    public static WebApplication UseReportsSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Reports API v1");
            options.RoutePrefix = "swagger";
            options.DocumentTitle = "PracticalWork.Reports API";
            options.DisplayRequestDuration();
            options.EnableFilter();
        });

        return app;
    }

    private static void IncludeAllXmlComments(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
    {
        var baseDir = AppContext.BaseDirectory;
        foreach (var xml in Directory.EnumerateFiles(baseDir, "*.xml"))
            options.IncludeXmlComments(xml, includeControllerXmlComments: true);

        var assemblyName = Assembly.GetExecutingAssembly().GetName().Name!;
        var sibling = Path.Combine(baseDir, $"{assemblyName}.xml");
        if (File.Exists(sibling))
            options.IncludeXmlComments(sibling, includeControllerXmlComments: true);
    }
}
