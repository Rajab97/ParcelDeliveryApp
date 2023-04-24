using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Serilog;
using SharedLibrary.Models.Commons;
using System.Net;

namespace UserManagementService.Middlewares
{
    public class ApiKeyMiddleware : IMiddleware
    {
        private readonly AppConfig _appConfig;

        public ApiKeyMiddleware(IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                if (context.Request.Path.StartsWithSegments(_appConfig.ExposedEndpointPrefix))
                {
                    if (context.Request.Headers.TryGetValue("ApiKey",out StringValues apiKey))
                    {
                        if (apiKey != _appConfig.ApiKey)
                        {
                            await UnAuthorizeRequest(context);
                            return;
                        }
                    }
                    else
                    {
                        await UnAuthorizeRequest(context);
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error occured while process ApiKeyMiddleware");
                await UnAuthorizeRequest(context);
                return;
            }

            await next(context);
        }

        private async Task UnAuthorizeRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResponse.Error("Invalid API Key")));
        }
    }
}
