using AutoMapper;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderManagementService.Application.DTOs.Request;
using OrderManagementService.Application.Services;
using OrderManagementService.Helpers.Configs;
using SharedLibrary.Models.KafkaSchemaRegistry;

namespace OrderManagementService.Infrastructure.Services.Background
{
    public class GetLastOrderStatusBackgroundService : BackgroundService
    {
        private readonly ILogger<GetLastOrderStatusBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaConfig _kafkaConfig;
        private readonly IConsumer<Ignore, string> _consumer;

        public GetLastOrderStatusBackgroundService(ILogger<GetLastOrderStatusBackgroundService> logger,
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
                        var _orderHandlerService = services.GetRequiredService<IOderRegistrationService>();
                        var _mapper = services.GetRequiredService<IMapper>();

                        try
                        {
                            _logger.LogInformation($"Start subscribe to {KafkaTopics.ORDER_STATUS_CHANGED}");
                            _consumer.Subscribe(KafkaTopics.ORDER_STATUS_CHANGED);
                            _logger.LogInformation($"Successfully subscribed to {KafkaTopics.ORDER_STATUS_CHANGED}");

                            while (!stoppingToken.IsCancellationRequested)
                            {
                                try
                                {
                                    var result = _consumer.Consume(stoppingToken);
                                    _logger.LogInformation($"Received message: {result.Value}");
                                    var message = JsonConvert.DeserializeObject<OrderStatusChangedMessage>(result.Value);
                                    var dto = _mapper.Map<ChangeOrderStatusRequestDTO>(message);
                                    var response = await _orderHandlerService.ChangeOrderStatusAsync(dto);
                                    if (!response.Succeeded)
                                        _logger.LogError(response.ErrorMessage);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError(e, $"Error occured while consume {KafkaTopics.ASSIGN_TO_COURIER}");
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, $"Error occured while subscribe to {KafkaTopics.ASSIGN_TO_COURIER}");
                            await Task.Delay(5000);
                        }
                    }
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
