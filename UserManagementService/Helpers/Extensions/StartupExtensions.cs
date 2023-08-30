using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SharedLibrary.Domain.Repositories;
using System.Text;
using UserManagementService.Application.Services;
using UserManagementService.Application.Services.Common;
using UserManagementService.Domain;
using UserManagementService.Domain.Entities;
using UserManagementService.Helpers.Configs;
using UserManagementService.Identity.Statics;
using UserManagementService.Infrastructure.Persistance;
using UserManagementService.Infrastructure.Persistance.Repositories;
using UserManagementService.Infrastructure.Services;
using UserManagementService.Middlewares;

namespace UserManagementService.Helpers.Extensions
{
    public static class StartupExtensions
    {
        public static IServiceCollection AddMyAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtConfig:SecretKey"])),
                       ValidateIssuer = false,
                       ValidateAudience = false,
                       RequireExpirationTime = true
                   };
       });

            services.AddAuthorization();
            return services;
        }
        public static IServiceCollection AddExternalPackages(this IServiceCollection services,IConfiguration configuration)
        {
            string connectionString = configuration.GetValue<string>("DataBaseConfig:ConnectionString");
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(connectionString);
            });
            //User Microsoft identity for authentication and authorization
            services.AddIdentity<User, IdentityRole<int>>(options => {
                //options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                //options.SignIn.RequireConfirmedPhoneNumber = true;
                options.User.RequireUniqueEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


            //User automapper for mapping DTO models to Entities
            services.AddAutoMapper(typeof(Program).Assembly);

            //Use FleuntValidation for validate request models
            services.AddValidatorsFromAssemblyContaining<Program>();

            //HealtCheck configuration
            services.AddHealthChecks()
                    .AddSqlServer(connectionString, name: "SqlServer healt check",
                        failureStatus: HealthStatus.Degraded, tags: new[] { "db", "sql" });
            return services;
        }
        public static IServiceCollection AddIdentityServerConfiguration(this IServiceCollection services)
        {
            services.AddIdentityServer(options =>
            {
                //For extra configurations
            })
                .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
                .AddInMemoryClients(IdentityConfig.Clients)
                .AddInMemoryIdentityResources(IdentityConfig.IdentityResources)
                .AddAspNetIdentity<User>();
            return services;
        }
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region Application services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
            #endregion

            #region Utility Services
            services.AddSingleton<IItentityHelperService, ItentityHelperService>();
            services.AddScoped<ExceptionHandlingMiddleware>();
            services.AddScoped<ApiKeyMiddleware>();
            #endregion
            return services;
        }
        public static IServiceCollection AddAppsettingsConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JWTConfig>(configuration.GetSection("JwtConfig"));
            services.Configure<DefaultUserConfig>(configuration.GetSection("DefaultUserConfig"));
            services.Configure<AppConfig>(configuration.GetSection("AppConfig"));
            return services;
        }

        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ExceptionHandlingMiddleware>();
            return applicationBuilder;
        } 
        public static IApplicationBuilder UseApiKeyAuthenticationForExposedRoutes(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ApiKeyMiddleware>();
            return applicationBuilder;
        }
        public static void ConfiureSerilog(this WebApplicationBuilder appBuilder)
        {
            Log.Logger = new LoggerConfiguration()
                       .Enrich.FromLogContext()
                       .Enrich.WithProperty("Environment", appBuilder.Environment.EnvironmentName)
                       .ReadFrom.Configuration(appBuilder.Configuration)
                       .WriteTo.Console()
                       .CreateLogger();
            Log.Information("UserManagementService is starting...");
        }
    }
}
