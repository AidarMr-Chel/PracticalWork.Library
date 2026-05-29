using Microsoft.EntityFrameworkCore;
using Npgsql;
using PracticalWork.Library;
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

namespace PracticalWork.Library.Web.Infrastructure;

/// <summary>
/// Регистрация сервисов приложения библиотеки.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLibraryApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPostgreSqlStorage(cfg =>
        {
            var dataSource = new NpgsqlDataSourceBuilder(configuration["App:DbConnectionString"])
                .EnableDynamicJson()
                .Build();

            cfg.UseNpgsql(dataSource);
        });

        services.AddDbContext<ReportsDbContext>(options =>
        {
            var dataSource = new NpgsqlDataSourceBuilder(configuration["App:ReportsDbConnectionString"])
                .EnableDynamicJson()
                .Build();

            options.UseNpgsql(dataSource);
        });

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

        services.AddScoped<IReminderRepository, ReminderRepository>();
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
        services.AddScoped<INotificationLogRepository, NotificationLogRepository>();

        services.AddSingleton<IEmailTemplateService, FileEmailTemplateService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddSingleton<TimeProvider>(_ => TimeProvider.System);

        services.AddDomain();
        services.AddCache(configuration);
        services.AddMinioFileStorage(configuration);
        services.AddRabbitMqPublisher(configuration);
        services.AddJobs(configuration);

        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<JobSettings>(configuration.GetSection("JobSettings"));
        services.Configure<ArchiveSettings>(configuration.GetSection("ArchiveSettings"));
        services.Configure<EmailTemplateSettings>(configuration.GetSection("EmailTemplateSettings"));
        services.Configure<MinioReportsSettings>(configuration.GetSection("MinioReportsSettings"));

        return services;
    }
}
