using ApiGateway.Middlewares;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using SharedLibrary.Models.Commons;
using System.Net;
using System.Reflection.PortableExecutable;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath)
        .AddJsonFile("ocelot.json", optional:false,reloadOnChange:true)
        .AddEnvironmentVariables();
builder.Services.AddOcelot(builder.Configuration);

Log.Logger = new LoggerConfiguration()
.Enrich.FromLogContext()
             .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
             .ReadFrom.Configuration(builder.Configuration)
             .WriteTo.Console()
             .CreateLogger();

Log.Information("ApiGateway is starting...");
builder.Host.UseSerilog();
var app = builder.Build();

app.UseSerilogRequestLogging();

var configuration = new OcelotPipelineConfiguration
{
    PreErrorResponderMiddleware = async (context, next) =>
    {
        if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("/api/ex"))
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResponse.Error("Not allowed from outside")));
            return;
        }

        await next();
    }
};

await app.UseOcelot(configuration);
app.Run();
