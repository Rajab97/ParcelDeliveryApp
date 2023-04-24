using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderManagementService.Helpers;
using OrderManagementService.Helpers.Extensions;
using OrderManagementService.Infrastructure.Persistance;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Application services
builder.Services.AddExternalPackages(builder.Configuration)
.AddApplicationServices()
                .AddAppsettingsConfigs(builder.Configuration)
                .AddBackgroundServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
//Disable default model state validation for Validation with FluentValidation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();
#region Configure Logging
builder.Host.UseSerilog();
builder.ConfiureSerilog();
#endregion

var app = builder.Build();

#region Initialise and Seed data
await app.InitialiseKafkaTopics(builder.Configuration);
#endregion

#region Middlewares
//Use custom middleware for logging exception and response with generic ApiResponse model

app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});
#endregion

app.Run();
