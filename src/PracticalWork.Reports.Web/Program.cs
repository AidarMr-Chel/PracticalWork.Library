using PracticalWork.Reports.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReportsSwagger();
builder.Services.AddReportsApplication(builder.Configuration);

var app = builder.Build();

app.UseReportsSwagger();
app.MapControllers();

app.Run();
