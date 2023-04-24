using Newtonsoft.Json;
using Serilog;
using SharedLibrary.Models.Commons;
using SharedLibrary.Models.Constants;
using System.Net;

namespace DeliveryManagementService.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ApplicationException ex)
            {
                Log.Error(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResponse.Error(ex.Message)));
            }
            catch (Exception ex)
            {
                Log.Error(ex,ExceptionMessages.ExceptionOccured);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsync(JsonConvert.SerializeObject(ApiResponse.Error(ExceptionMessages.ExceptionOccured)));
            }
        }
    }
}
