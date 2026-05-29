using PracticalWork.Reports.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddReportsApplication(builder.Configuration);

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
