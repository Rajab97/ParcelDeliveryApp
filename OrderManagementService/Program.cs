using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
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
                .AddMyAuthentication(builder.Configuration)
                .AddAppsettingsConfigs(builder.Configuration)
                .AddHttpClients(builder.Configuration)
                .AddBackgroundServices();

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();
//Disable default model state validation for Validation with FluentValidation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => SwaggerHelper.ConfigureSwaggerGen(c));
#region Configure Logging
builder.Host.UseSerilog();
builder.ConfiureSerilog();
#endregion

var app = builder.Build();

 #region Initialise and Seed data
 await app.InitialiseAsync();
 await app.SeedAsync();
 #endregion

 #region Middlewares
 if (app.Environment.IsDevelopment())
 {
     app.UseSwagger(c => SwaggerHelper.ConfigureSwagger(c));
     app.UseSwaggerUI(c => SwaggerHelper.ConfigureSwaggerUI(c));
 }
 //Use custom middleware for logging exception and response with generic ApiResponse model
 app.UseCustomExceptionHandler();
 app.UseHttpsRedirection();

 app.UseAuthentication();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
    endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
});
#endregion

app.Run();
