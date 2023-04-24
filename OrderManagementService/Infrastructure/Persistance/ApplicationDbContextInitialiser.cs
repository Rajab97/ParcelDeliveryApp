using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using SharedLibrary.Models.Auth;

namespace OrderManagementService.Infrastructure.Persistance
{
    public static class ApplicationDbContextInitialiser
    {
        public static async Task InitialiseAsync(this IApplicationBuilder app)
        {
            Log.Information("InitialiseAsync method called");

            using (var scope = app.ApplicationServices.CreateScope())
            {
                try
                {
                    var services = scope.ServiceProvider;
                    var dbContext = services.GetRequiredService<ApplicationDbContext>();

                    Log.Information("Successfully connected to server");
                    dbContext.Database.Migrate();
                    Log.Information("Successfully migrated");
                }
                catch (Exception exc)
                {
                    Log.Error(exc, exc.Message);
                    throw;
                }
            }
           
        }

        public static async Task SeedAsync(this IApplicationBuilder app)
        {
            Log.Information("SeedAsync method called");

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
              
               
            }
        }
    }
}
