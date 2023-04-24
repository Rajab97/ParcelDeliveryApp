﻿using Confluent.Kafka.Admin;
using Confluent.Kafka;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderManagementService.Application.Common.Constants;
using OrderManagementService.Application.Services;
using OrderManagementService.Helpers.Configs;
using OrderManagementService.Helpers.Models;
using OrderManagementService.Infrastructure.Persistance;
using OrderManagementService.Infrastructure.Persistance.Repositories;
using OrderManagementService.Infrastructure.Services;
using OrderManagementService.Middlewares;
using Serilog;
using SharedLibrary.Domain;
using SharedLibrary.Domain.Repositories;
using SharedLibrary.Models.Auth;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using OrderManagementService.Infrastructure.Services.Common;
using SharedLibrary.Models.KafkaSchemaRegistry;
using OrderManagementService.Application.Services.Common;
using OrderManagementService.Infrastructure.Services.Background;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace OrderManagementService.Helpers.Extensions
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
            //Register client for UserManagementService
            string baseUrlUser = configuration.GetValue<string>("AppConfig:UserManagementBaseUrl");
            string apiKeyUser = configuration.GetValue<string>("AppConfig:ApiKey");
            services.AddHttpClient<IAccountService,AccountService>(options => {
                options.BaseAddress = new Uri(baseUrlUser);
                options.DefaultRequestHeaders.Add("ApiKey", apiKeyUser);
                options.DefaultRequestHeaders.Add("Accept", "application/json");
                options.Timeout = TimeSpan.FromSeconds(60);
            });

            //Register client for DeliveryManagementService
            string baseUrlDelivery = configuration.GetValue<string>("AppConfig:DeliveryManagementBaseUrl");
            string apiKeyDelivery = configuration.GetValue<string>("AppConfig:ApiKey");
            services.AddHttpClient<IDeliveryService, DeliveryService>(options => {
                options.BaseAddress = new Uri(baseUrlDelivery);
                options.DefaultRequestHeaders.Add("ApiKey", apiKeyDelivery);
                options.DefaultRequestHeaders.Add("Accept", "application/json");
                options.Timeout = TimeSpan.FromSeconds(60);
            });
            return services;
        }
        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<GetLastOrderStatusBackgroundService>();
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
            services.AddScoped<IOderRegistrationService, OderRegistrationService>();
            #endregion

            #region Utility Services
            services.AddScoped<IKafkaHelperService, KafkaHelperService>();
            services.AddScoped<ExceptionHandlingMiddleware>();
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

        public static async Task InitialiseKafkaTopics(this IApplicationBuilder app, IConfiguration configuration)
        {
            Log.Information("InitialiseKafkaTopics method called");
            using (var scope = app.ApplicationServices.CreateScope())
            {
                try
                {
                    var services = scope.ServiceProvider;
                    var kafkaConfig = services.GetRequiredService<IOptions<KafkaConfig>>().Value;
                    // create Kafka topic if it does not exist
                    var config = new AdminClientConfig { BootstrapServers = kafkaConfig.Host };
                    using (var adminClient = new AdminClientBuilder(config).Build())
                    {
                        foreach (var topic in KafkaTopics.GetAvailableTopics())
                        {
                            try
                            {
                                Log.Information($"Try to create {topic} topic");
                                var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
                                var topics = metadata.Topics;
                                if (topics.FirstOrDefault(t => t.Topic == topic) == null)
                                {
                                    Log.Information($"{topic} topic not exist yet");
                                    var topicSpec = new TopicSpecification
                                    {
                                        Name = topic,
                                        NumPartitions = 1,
                                        ReplicationFactor = 1
                                    };
                                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> { topicSpec });
                                    Log.Information($"{topic} topic created successfully");
                                }
                                else
                                    Log.Information($"{topic} topic already exist");
                            }
                            catch (Exception e)
                            {
                                Log.Error(e,$"Error occured while create {topic} topic");
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    Log.Error(exc, exc.Message);
                    throw;
                }
            }
        }

        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.UseMiddleware<ExceptionHandlingMiddleware>();
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
            Log.Information("OrderManagementService is starting...");
        }
    }
}