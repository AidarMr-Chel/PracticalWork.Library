using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.AspNetCore;
using PracticalWork.Library.Models;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Services;

namespace PracticalWork.Library.Infrastructure.Jobs;

/// <summary>
/// Точка входа для регистрации Quartz‑задач и связанных воркфлоу.
/// Отвечает за настройку DI, Quartz и загрузку расписаний из конфигурации.
/// </summary>
public static class Entry
{
    /// <summary>
    /// Регистрирует Quartz‑задачи, воркфлоу и настраивает Quartz‑планировщик.
    /// </summary>
    /// <param name="services">Коллекция сервисов DI.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    /// <returns>Обновлённая коллекция сервисов.</returns>
    public static IServiceCollection AddJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Регистрация воркфлоу
        services.AddScoped<ArchiveBooksWorkflow>();
        services.AddScoped<ReturnReminderWorkflow>();

        // Настройка Quartz
        services.AddQuartz(q =>
        {
            ConfigurePersistentStore(q, configuration);
            RegisterJobsFromConfig(q, configuration);
        });

        // Хост-сервис для запуска Quartz
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

        return services;
    }

    /// <summary>
    /// Настраивает Quartz для использования постоянного хранилища (PostgreSQL).
    /// </summary>
    /// <param name="q">Конфигуратор Quartz.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    private static void ConfigurePersistentStore(
        IServiceCollectionQuartzConfigurator q,
        IConfiguration configuration)
    {
        var connectionString = configuration["App:DbConnectionString"]
            ?? throw new InvalidOperationException("App:DbConnectionString not configured");

        q.UsePersistentStore(s =>
        {
            s.UseProperties = true;
            s.UsePostgres(connectionString);
            s.UseNewtonsoftJsonSerializer();
            s.Properties["quartz.jobStore.tablePrefix"] = "qrtz_";
        });
    }

    /// <summary>
    /// Регистрирует Quartz‑задачи и триггеры на основе конфигурации.
    /// </summary>
    /// <param name="q">Конфигуратор Quartz.</param>
    /// <param name="configuration">Конфигурация приложения.</param>
    private static void RegisterJobsFromConfig(
        IServiceCollectionQuartzConfigurator q,
        IConfiguration configuration)
    {
        var jobSettings = configuration
            .GetSection("JobSettings:Jobs")
            .Get<Dictionary<string, JobConfiguration>>()
            ?? new Dictionary<string, JobConfiguration>();

        foreach (var (jobKey, config) in jobSettings)
        {
            var jobType = ResolveJobType(jobKey);
            var key = new JobKey(jobKey, "LibraryJobs");

            q.AddJob(jobType, key, cfg => cfg.StoreDurably());

            q.AddTrigger(cfg => cfg
                .ForJob(key)
                .WithIdentity($"{jobKey}-trigger", "LibraryJobs")
                .WithCronSchedule(config.CronExpression, x =>
                {
                    x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time"));
                    x.WithMisfireHandlingInstructionFireAndProceed();
                }));
        }
    }

    /// <summary>
    /// Определяет тип Quartz‑задачи по ключу из конфигурации.
    /// </summary>
    /// <param name="jobKey">Ключ задачи.</param>
    /// <returns>Тип задачи.</returns>
    /// <exception cref="InvalidOperationException">Если задача не найдена.</exception>
    private static Type ResolveJobType(string jobKey) => jobKey switch
    {
        "ReturnReminder" => typeof(ReturnReminderJob),
        "WeeklyReport" => typeof(WeeklyReportJob),
        "ArchiveBooks" => typeof(ArchiveBooksJob),
        _ => throw new InvalidOperationException($"Unknown job: {jobKey}")
    };
}
