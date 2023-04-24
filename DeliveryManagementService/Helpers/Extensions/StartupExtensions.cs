using Confluent.Kafka.Admin;
using Confluent.Kafka;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SharedLibrary.Domain;
using SharedLibrary.Domain.Repositories;
using SharedLibrary.Models.Auth;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using SharedLibrary.Models.KafkaSchemaRegistry;
using DeliveryManagementService.Application.Common.Constants;
using DeliveryManagementService.Infrastructure.Persistance;
using DeliveryManagementService.Infrastructure.Persistance.Repositories;
using DeliveryManagementService.Helpers.Models;
using DeliveryManagementService.Application.Services.Common;
using DeliveryManagementService.Infrastructure.Services.Common;
using DeliveryManagementService.Infrastructure.Services.Background;
using DeliveryManagementService.Application.Services;
using DeliveryManagementService.Infrastructure.Services;
using DeliveryManagementService.Middlewares;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace DeliveryManagementService.Helpers.Extensions
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

            services.AddAuthorization(options => {
                options.AddPolicy(CustomPolicyTypes.OnlyAdminPolicy,m=> m.RequireRole(RoleTypes.Admin));
                options.AddPolicy(CustomPolicyTypes.UserPolicy,m=> m.RequireRole(RoleTypes.Admin,RoleTypes.User));
                options.AddPolicy(CustomPolicyTypes.CourierPolicy,m=> m.RequireRole(RoleTypes.Admin,RoleTypes.Courier));
                options.AddPolicy(CustomPolicyTypes.AllUsersPolicy,m=> m.RequireAuthenticatedUser());
            });
            return services;
        }
        public static IServiceCollection AddHttpClients(this IServiceCollection services, IConfiguration configuration)
        {
           /* string baseUrl = configuration.GetValue<string>("AppConfig:UserManagementBaseUrl");
            string apiKey = configuration.GetValue<string>("AppConfig:UserManagementApiKey");
            services.AddHttpClient<IAccountService,AccountService>(options => {
                options.BaseAddress = new Uri(baseUrl);
                options.DefaultRequestHeaders.Add("ApiKey", apiKey);
                options.DefaultRequestHeaders.Add("Accept", "application/json");
                options.Timeout = TimeSpan.FromSeconds(60);
            });*/
            return services;
        }
        public static IServiceCollection AddExternalPackages(this IServiceCollection services,IConfiguration configuration)
        {
            string connectionString = configuration.GetValue<string>("DataBaseConfig:ConnectionString");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            //User automapper for mapping DTO models to Entities
            services.AddAutoMapper(typeof(Program).Assembly);

            //Use FleuntValidation for validate request models
            services.AddValidatorsFromAssemblyContaining<Program>();

            //HealtCheck configuration
            var kafkaHost = configuration.GetValue<string>("KafkaConfig:Host");
            services.AddHealthChecks()
                    .AddSqlServer(connectionString, name: "SqlServer healt check",
                        failureStatus: HealthStatus.Degraded, tags: new[] { "db", "sql" })
                    .AddKafka(new ProducerConfig
                    {
                        BootstrapServers = kafkaHost,
                        SocketConnectionSetupTimeoutMs = 5000
                        // Other configuration properties as needed
                    }, tags: new[] { "kafka" });


            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region Application services
            services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IOrderHandlerService, OrderHandlerService>();
            services.AddScoped<IDeliveryHistoryService, DeliveryHistoryService>();
            #endregion

            #region Utility Services
            services.AddScoped<IKafkaHelperService, KafkaHelperService>();
            services.AddScoped<ExceptionHandlingMiddleware>();
            services.AddScoped<ApiKeyMiddleware>();
            services.AddScoped<CurrentUser>();
            #endregion
            return services;
        }
        public static IServiceCollection AddAppsettingsConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JWTConfig>(configuration.GetSection("JwtConfig"));
            services.Configure<KafkaConfig>(configuration.GetSection("KafkaConfig"));
            services.Configure<AppConfig>(configuration.GetSection("AppConfig"));
            return services;
        }
        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<GetAssignedOrdersBackgroundService>();
            services.AddHostedService<CancelOrderBackgroundService>();
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
            Log.Information("DeliveryManagementService is starting...");
        }
    }
}
