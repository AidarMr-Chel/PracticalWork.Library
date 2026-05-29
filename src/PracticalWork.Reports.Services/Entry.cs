using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Reports.Services.Abstractions;

namespace PracticalWork.Reports.Services;

/// <summary>
/// Регистрация сервисов модуля отчётов.
/// </summary>
public static class Entry
{
    public static IServiceCollection AddReportsServices(this IServiceCollection services)
    {
        services.AddScoped<IActivityLogService, ActivityLogService>();
        services.AddScoped<IActivityLogIngestionService, ActivityLogIngestionService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}
