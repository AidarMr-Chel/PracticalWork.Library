using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Cache.Redis;
using PracticalWork.Reports.Data.PostgreSql;
using PracticalWork.Reports.MessageBroker;
using PracticalWork.Reports.Minio;
using PracticalWork.Reports.Services;
using PracticalWork.Reports.Web.Validations;

namespace PracticalWork.Reports.Web.Infrastructure;

/// <summary>
/// Регистрация сервисов приложения отчётов.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReportsApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ReportsDbContext>(options =>
            options.UseNpgsql(configuration["App:DbConnectionString"]));

        services.AddReportsCache(configuration);
        services.AddMinioModule(configuration);
        services.AddReportsPostgreSql();
        services.AddReportsServices();
        services.AddReportsMessageBroker(configuration);

        services.AddValidatorsFromAssemblyContaining<GenerateReportRequestValidator>();
        services.AddFluentValidationAutoValidation();

        services.AddControllers();

        return services;
    }
}
