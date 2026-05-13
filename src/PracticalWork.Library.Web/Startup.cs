using JetBrains.Annotations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using Quartz;
using PracticalWork.Library.Abstractions.Services;
using PracticalWork.Library.Abstractions.Storage;
using PracticalWork.Library.Models;
using PracticalWork.Library.Cache.Redis;
using PracticalWork.Library.Controllers;
using PracticalWork.Library.Data.Minio;
using PracticalWork.Library.Data.PostgreSql;
using PracticalWork.Library.Data.PostgreSql.Repositories;
using PracticalWork.Library.Exceptions;
using PracticalWork.Library.Infrastructure.Jobs;
using PracticalWork.Library.MessageBroker;
using PracticalWork.Library.Services;
using PracticalWork.Library.Web.Configuration;
using System.Net.Sockets;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace PracticalWork.Library.Web;

public class Startup
{
    private static string _basePath;
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        _basePath = string.IsNullOrWhiteSpace(Configuration["GlobalPrefix"])
            ? ""
            : $"/{Configuration["GlobalPrefix"].Trim('/')}";
    }

    public void ConfigureServices(IServiceCollection services)
    {

        services.AddPostgreSqlStorage(cfg =>
        {
            var npgsqlDataSource = new NpgsqlDataSourceBuilder(
                    Configuration["App:DbConnectionString"])
                .EnableDynamicJson()
                .Build();

            cfg.UseNpgsql(npgsqlDataSource);
        });

        services.Configure<ArchiveSettings>(Configuration.GetSection("ArchiveSettings"));

        services.AddDbContext<PracticalWork.Library.Data.PostgreSql.ReportsDbContext>(options =>
        {
            var npgsqlDataSource = new NpgsqlDataSourceBuilder(
                    Configuration["App:ReportsDbConnectionString"])
                .EnableDynamicJson()
                .Build();

            options.UseNpgsql(npgsqlDataSource);
        });
        services.Configure<MinioReportsSettings>(
            Configuration.GetSection("MinioReportsSettings"));

        services.AddMvc(opt =>
        {
            opt.Filters.Add<DomainExceptionFilter<AppException>>();
        })
        .AddApi()
        .AddControllersAsServices()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(
                new JsonStringEnumConverter());

            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        });

        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.FullName);

            c.UseOneOfForPolymorphism();

            c.IncludeXmlComments(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "PracticalWork.Library.Contracts.xml"));

            c.IncludeXmlComments(
                Path.Combine(
                    AppContext.BaseDirectory,
                    "PracticalWork.Library.Controllers.xml"));
        });


        services.AddDomain();

        services.AddCache(Configuration);

        services.AddMinioFileStorage(Configuration);

        services.AddRabbitMqPublisher(Configuration);


        services.Configure<EmailSettings>(
            Configuration.GetSection("EmailSettings"));

        services.Configure<JobSettings>(
            Configuration.GetSection("JobSettings"));

        services.Configure<ArchiveSettings>(
            Configuration.GetSection("ArchiveSettings"));

        services.Configure<EmailTemplateSettings>(
            Configuration.GetSection("EmailTemplateSettings"));


        services.AddScoped<IEmailService, SmtpEmailService>();


        services.AddScoped<INotificationLogRepository, NotificationLogRepository>();


        services.AddQuartz(q =>
        {
            var connectionString =
                Configuration["App:DbConnectionString"];


            q.UsePersistentStore(s =>
            {
                s.UseProperties = true;

                s.UsePostgres(connectionString);

                s.UseNewtonsoftJsonSerializer();

                s.Properties["quartz.jobStore.tablePrefix"] = "qrtz_";
            });


            var jobSettings =
                Configuration
                    .GetSection("JobSettings:Jobs")
                    .Get<Dictionary<string, JobConfiguration>>()
                ?? new Dictionary<string, JobConfiguration>();


            foreach (var (jobKey, config) in jobSettings)
            {

                var jobType = jobKey switch
                {
                    "ReturnReminder" => typeof(ReturnReminderJob),

                    "WeeklyReport" => typeof(WeeklyReportJob),

                    "ArchiveBooks" => typeof(ArchiveBooksJob),

                    _ => throw new InvalidOperationException(
                        $"Unknown job: {jobKey}")
                };


                var jobName = jobKey;

                var jobGroup = "LibraryJobs";

                var triggerName = $"{jobKey}-trigger";

                var triggerGroup = "LibraryJobs";


                var key = new JobKey(jobName, jobGroup);


                q.AddJob(
                    jobType,
                    key,
                    cfg =>
                    {
                        cfg.StoreDurably();
                    });


                q.AddTrigger(cfg => cfg
                    .ForJob(key)

                    .WithIdentity(
                        triggerName,
                        triggerGroup)

                    .WithCronSchedule(
                        config.CronExpression,
                        x =>
                        {
                            x.InTimeZone(
                                TimeZoneInfo.FindSystemTimeZoneById(
                                    "Russian Standard Time"));

                            x.WithMisfireHandlingInstructionFireAndProceed();
                        }));
            }

        });


        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
    }

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
                var descriptions =
                    endpoints.DescribeApiVersions();

                foreach (var description in descriptions)
                {
                    var url =
                        $"/swagger/{description.GroupName}/swagger.json";

                    var name =
                        description.GroupName.ToUpperInvariant();

                    options.SwaggerEndpoint(url, name);
                }
            });

            endpoints.MapControllers();
        });
    }
}