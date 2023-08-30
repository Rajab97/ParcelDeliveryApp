using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Text.Json.Serialization;
using UserManagementService.Helpers;
using UserManagementService.Helpers.Extensions;
using UserManagementService.Infrastructure.Persistance;
var builder = WebApplication.CreateBuilder(args);

//Application services
builder.Services.AddExternalPackages(builder.Configuration)
                .AddApplicationServices()
                .AddIdentityServerConfiguration()
                .AddMyAuthentication(builder.Configuration)
                .AddAppsettingsConfigs(builder.Configuration);

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
//await app.InitialiseAsync();
//await app.SeedAsync();
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

app.UseApiKeyAuthenticationForExposedRoutes();
//Not need call to UseAuthentication because it already called in UseIDentity
//app.UseAuthentication();

app.UseRouting();

app.UseIdentityServer();    
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
