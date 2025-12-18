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
builder.Services.AddSwaggerGen();

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
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
