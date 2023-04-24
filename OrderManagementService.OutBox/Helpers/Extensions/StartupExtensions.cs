using Confluent.Kafka.Admin;
using Confluent.Kafka;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderManagementService.Application.Services;
using OrderManagementService.Helpers.Configs;
using OrderManagementService.Infrastructure.Persistance;
using OrderManagementService.Infrastructure.Persistance.Repositories;
using OrderManagementService.Infrastructure.Services;
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
        public static IServiceCollection AddBackgroundServices(this IServiceCollection services)
        {
            services.AddHostedService<ProduceOutBoxMessagesBackgroundService>();
            return services;
        }
        public static IServiceCollection AddExternalPackages(this IServiceCollection services,IConfiguration configuration)
        {
            string connectionString = configuration.GetValue<string>("DataBaseConfig:ConnectionString");
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            #region Application services
            services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            #region Utility Services
            services.AddScoped<IKafkaHelperService, KafkaHelperService>();
            #endregion
            return services;
        }
        public static IServiceCollection AddAppsettingsConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaConfig>(configuration.GetSection("KafkaConfig"));
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

        public static void ConfiureSerilog(this WebApplicationBuilder appBuilder)
        {
            Log.Logger = new LoggerConfiguration()
                       .Enrich.FromLogContext()
                       .Enrich.WithProperty("Environment", appBuilder.Environment.EnvironmentName)
                       .ReadFrom.Configuration(appBuilder.Configuration)
                       .WriteTo.Console()
                       .CreateLogger();
            Log.Information("OrderManagementService.OutBox is starting...");
        }
    }
}
