using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderManagementService.Application.Services;
using OrderManagementService.Application.Services.Common;
using OrderManagementService.Domain.Entities;
using OrderManagementService.Helpers.Configs;
using SharedLibrary.Domain;
using SharedLibrary.Domain.Enums;
using SharedLibrary.Domain.Repositories;
using SharedLibrary.Models.KafkaSchemaRegistry;

namespace OrderManagementService.Infrastructure.Services.Background
{
    public class ProduceOutBoxMessagesBackgroundService : BackgroundService
    {
        private readonly ILogger<ProduceOutBoxMessagesBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaConfig _kafkaConfig;
        private readonly IConsumer<Ignore, string> _consumer;

        public ProduceOutBoxMessagesBackgroundService(ILogger<ProduceOutBoxMessagesBackgroundService> logger,
                                                    IOptions<KafkaConfig> kafkaConfig, IServiceProvider serviceProvider
                                                     )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _kafkaConfig = kafkaConfig.Value;
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaConfig.Host,
                GroupId = _kafkaConfig.GroupName,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            _consumer = new ConsumerBuilder<Ignore, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError($"Error happend while consume message: {e.Code} {e.Reason}"))
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Run(async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var services = scope.ServiceProvider;
                        var _outBoxRepo = services.GetRequiredService<IRepository<OutBoxMessage>>();
                        var _kafkaHelperService = services.GetRequiredService<IKafkaHelperService>();
                        var _unitOfWork = services.GetRequiredService<IUnitOfWork>();

                        try
                        {
                            _logger.LogInformation($"Start retrive pending and failed messages");
                            var messages = await _outBoxRepo.GetIQueryable().Where(m => m.Status == OutBoxStatusTypes.Failed.ToString() ||
                                                                                m.Status == OutBoxStatusTypes.Pending.ToString()).ToListAsync();


                            _logger.LogInformation($"{messages.Count} messages retrived");

                            foreach (var message in messages)
                            {
                                try
                                {
                                    try
                                    {
                                        //Consumer side must be idompitent for dubblicate messages
                                        await _kafkaHelperService.ProduceMessageAsync(message.Message, message.Topic);
                                        message.SentDate = DateTime.Now;
                                        message.Status = OutBoxStatusTypes.Delivered.ToString();
                                    }
                                    catch (Exception e)
                                    {
                                        message.Status = OutBoxStatusTypes.Failed.ToString();
                                        _logger.LogError(e, $"Error occured while produce message to {message.Topic} topic");
                                    }
                                    await _outBoxRepo.UpdateAsync(message);
                                    await _unitOfWork.CompleteAsync();
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, "Error occured while update database");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"Error occured while retriving outBox data");
                        }
                    }
                    await Task.Delay(1000);
                }
            }, stoppingToken);
        }

        public override void Dispose()
        {
            _consumer.Dispose();
            base.Dispose();
        }
    }
}
