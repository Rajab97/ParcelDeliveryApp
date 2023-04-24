using Newtonsoft.Json;
using SharedLibrary.Models.Commons;
using System.Net;

namespace ApiGateway.Middlewares
{
    public class RouteAuthorizationMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.HasValue && context.Request.Path.Value.Contains("/api/ex"))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResponse.Error("Not allowed from outside")));
                return;
            }

            await next(context);
        }
    }
}
