using JetBrains.Annotations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Cache.Redis;
using PracticalWork.Library.Controllers;
using PracticalWork.Library.Data.Minio;
using PracticalWork.Library.Data.PostgreSql;
using PracticalWork.Library.Data.PostgreSql.Repositories;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Infrastructure.Jobs;
using PracticalWork.Library.MessageBroker;
using PracticalWork.Library.Models;
using PracticalWork.Library.Services;
using PracticalWork.Library.Web.Configuration;
using System.Text.Json.Serialization;

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
        // =========================
        // PostgreSQL (основная БД)
        // =========================
        services.AddPostgreSqlStorage(cfg =>
        {
            var dataSource = new NpgsqlDataSourceBuilder(Configuration["App:DbConnectionString"])
                .EnableDynamicJson()
                .Build();

            cfg.UseNpgsql(dataSource);
        });

        // =========================
        // Reports DbContext (БД отчётов)
        // =========================
        services.AddDbContext<ReportsDbContext>(options =>
        {
            var dataSource = new NpgsqlDataSourceBuilder(Configuration["App:ReportsDbConnectionString"])
                .EnableDynamicJson()
                .Build();

            options.UseNpgsql(dataSource);
        });

        // =========================
        // MVC + Swagger
        // =========================
        services.AddMvc(opt =>
        {
            opt.Filters.Add<DomainExceptionFilter<AppException>>();
        })
        .AddApi()
        .AddControllersAsServices()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.FullName);
            c.UseOneOfForPolymorphism();
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PracticalWork.Library.Contracts.xml"));
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "PracticalWork.Library.Controllers.xml"));
        });

        // =========================
        // Репозитории
        // =========================
        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
        services.AddScoped<INotificationLogRepository, NotificationLogRepository>();

        // =========================
        // Сервисы
        // =========================
        services.AddSingleton<IEmailTemplateService, FileEmailTemplateService>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        // =========================
        // TimeProvider (тестируемость)
        // =========================
        services.AddSingleton<TimeProvider>(_ => TimeProvider.System);

        // =========================
        // Доменные сервисы
        // =========================
        services.AddDomain();

        // =========================
        // Инфраструктура
        // =========================
        services.AddCache(Configuration);
        services.AddMinioFileStorage(Configuration);
        services.AddRabbitMqPublisher(Configuration);

        // =========================
        // Фоновые задачи (Quartz)
        // =========================
        services.AddJobs(Configuration);

        // =========================
        // Настройки (IOptions<T>)
        // =========================
        services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));
        services.Configure<JobSettings>(Configuration.GetSection("JobSettings"));
        services.Configure<ArchiveSettings>(Configuration.GetSection("ArchiveSettings"));
        services.Configure<EmailTemplateSettings>(Configuration.GetSection("EmailTemplateSettings"));
        services.Configure<MinioReportsSettings>(Configuration.GetSection("MinioReportsSettings"));
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
