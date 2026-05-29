using JetBrains.Annotations;
using PracticalWork.Library.Web.Infrastructure;

namespace PracticalWork.Library.Web;

/// <summary>
/// Конфигурация приложения (DI, middleware, инфраструктура).
/// </summary>
public class Startup
{
    private static string _basePath = string.Empty;
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        _basePath = string.IsNullOrWhiteSpace(Configuration["GlobalPrefix"])
            ? string.Empty
            : $"/{Configuration["GlobalPrefix"]!.Trim('/')}";
    }

    /// <summary>
    /// Регистрирует сервисы, инфраструктуру, настройки и фоновые задачи.
    /// </summary>
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLibraryApplication(Configuration);
    }

    /// <summary>
    /// Конфигурирует middleware‑конвейер приложения.
    /// </summary>
    [UsedImplicitly]
    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env,
        IHostApplicationLifetime lifetime,
        ILogger logger,
        IServiceProvider serviceProvider)
    {
        app.UsePathBase(new PathString(_basePath));

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var descriptions = endpoints.DescribeApiVersions();
                foreach (var description in descriptions)
                {
                    var url = $"/swagger/{description.GroupName}/swagger.json";
                    var name = description.GroupName.ToUpperInvariant();
                    options.SwaggerEndpoint(url, name);
                }
            });

            endpoints.MapControllers();
        });
    }
}
