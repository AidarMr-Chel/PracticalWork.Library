using Microsoft.Extensions.DependencyInjection;
using PracticalWork.Reports.Data.PostgreSql.Repositories;
using PracticalWork.Reports.Entities.Abstractions;

namespace PracticalWork.Reports.Data.PostgreSql;

/// <summary>
/// Регистрация репозиториев модуля отчётов.
/// </summary>
public static class Entry
{
    public static IServiceCollection AddReportsPostgreSql(this IServiceCollection services)
    {
        services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();

        return services;
    }
}
