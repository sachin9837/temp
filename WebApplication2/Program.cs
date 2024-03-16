using Microsoft.OpenApi.Models; // Ensure you have this using directive
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using EventCopilotBot.Services;
using API.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Register Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});

// Register services
builder.Services.AddSingleton<ISecretManager, SecretManager>();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddSingleton<AppInsights>();

var app = builder.Build();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
});

app.UseHttpsRedirection();

//app.UseAuthorization();
app.MapControllers();

app.Run();
