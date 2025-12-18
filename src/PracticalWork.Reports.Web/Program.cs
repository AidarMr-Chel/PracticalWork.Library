using Microsoft.EntityFrameworkCore;
using PracticalWork.Reports.Cache.Redis;
using PracticalWork.Reports.Data.PostgreSql;
using PracticalWork.Reports.Data.PostgreSql.Repositories;
using PracticalWork.Reports.MessageBroker;
using PracticalWork.Reports.Minio;
using PracticalWork.Reports.Services;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "PracticalWork.Reports API",
        Version = "v1",
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);

    options.TagActionsBy(api =>
    {
        return new[] { api.ActionDescriptor.RouteValues["controller"] ?? "API" };
    });

    options.DocInclusionPredicate((name, api) => true);
});

builder.Services.AddDbContext<ReportsDbContext>(options =>
    options.UseNpgsql(config["App:DbConnectionString"]));

builder.Services.Configure<RabbitMqOptions>(
    config.GetSection("App:RabbitMQ"));

builder.Services.AddReportsCache(builder.Configuration);
builder.Services.AddMinioModule(builder.Configuration);

builder.Services.AddScoped<ActivityLogRepository>();
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddScoped<ReportService>();

builder.Services.AddHostedService<ReportsEventConsumer>();

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Reports API v1");
        options.RoutePrefix = "swagger";
        options.DisplayRequestDuration();
        options.EnableFilter();
    });
}

app.MapControllers();

app.Run();
