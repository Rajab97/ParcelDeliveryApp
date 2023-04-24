using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using SharedLibrary.Models.Auth;
using UserManagementService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using UserManagementService.Application.Services.Common;

namespace UserManagementService.Infrastructure.Persistance
{
    public static class ApplicationDbContextInitialiser
    {
        public static async Task InitialiseAsync(this IApplicationBuilder app)
        {
            Log.Information("InitialiseAsync method called");

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                try
                {
                    Log.Information("Successfully connected to server");
                    dbContext.Database.Migrate();
                    Log.Information("Successfully migrated");
                }
                catch (Exception e)
                {
                    Log.Error(e,"Error occured while migrate to database");
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
                var userManager = services.GetRequiredService<UserManager<User>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();
                var identityHelper = services.GetRequiredService<IItentityHelperService>();
                var defaultUser = services.GetRequiredService<IOptions<DefaultUserConfig>>().Value;

                foreach (var role in identityHelper.GetAvailableRoles())
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        Log.Information("Try to create role");
                        var roleResult = await roleManager.CreateAsync(new IdentityRole<int>() { Name = role });
                        if (roleResult.Succeeded)
                        {
                            Log.Information($"{role} created successfully");
                        }
                        else
                        {
                            Log.Error($"Error occured while create {role} role. {JsonConvert.SerializeObject(roleResult)}");
                            throw new ApplicationException($"{role} role cann't created");
                        }
                    }
                }

                //Get this data for decied if i need create default admin role if any exist we pass it
                var userInAdminRole = await userManager.GetUsersInRoleAsync(RoleTypes.Admin);
                if (!userInAdminRole.Any())
                {
                    var adminUser = new User()
                    {
                        FirstName = defaultUser.FirstName,
                        LastName = defaultUser.LastName,
                        UserName = defaultUser.UserName,
                        Email = defaultUser.Email,
                        CreatedAt = DateTime.Now
                    };
                    var userResult = await userManager.CreateAsync(adminUser, defaultUser.Password);
                    if (userResult.Succeeded)
                    {
                        var userAddResult = await userManager.AddToRoleAsync(adminUser, RoleTypes.Admin);
                        if (userAddResult.Succeeded)
                            Log.Information($"Admin created successfully");
                        else
                            Log.Information($"Error happend while add role to user {JsonConvert.SerializeObject(userAddResult)}");
                    }
                    else
                    {
                        Log.Error($"Error occured while create admin user. {JsonConvert.SerializeObject(userResult)}");
                        throw new ApplicationException($"Default admin role couldn't created");
                    }
                }

            }
        }
    }
}
