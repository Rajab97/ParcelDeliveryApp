using AutoMapper;
using Confluent.Kafka;
using DeliveryManagementService.Application.DTOs.Request;
using DeliveryManagementService.Application.Services;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using SharedLibrary.Models.KafkaSchemaRegistry;

namespace DeliveryManagementService.Infrastructure.Services.Background
{
    public class GetAssignedOrdersBackgroundService : BackgroundService
    {
        private readonly ILogger<GetAssignedOrdersBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly KafkaConfig _kafkaConfig;
        private readonly IConsumer<Ignore, string> _consumer;

        public GetAssignedOrdersBackgroundService(ILogger<GetAssignedOrdersBackgroundService> logger,
                                                    IOptions<KafkaConfig> kafkaConfig,IServiceProvider serviceProvider
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
                        var _orderHandlerService = services.GetRequiredService<IOrderHandlerService>();
                        var _mapper = services.GetRequiredService<IMapper>();

                        try
                        {
                            _logger.LogInformation($"Start subscribe to {KafkaTopics.ASSIGN_TO_COURIER}");
                            _consumer.Subscribe(KafkaTopics.ASSIGN_TO_COURIER);
                            _logger.LogInformation($"Successfully subscribed to {KafkaTopics.ASSIGN_TO_COURIER}");

                            while (!stoppingToken.IsCancellationRequested)
                            {
                                try
                                {
                                    var result = _consumer.Consume(stoppingToken);
                                    _logger.LogInformation($"Received message: {result.Value}");
                                    var message = JsonConvert.DeserializeObject<AssignToCurierMessage>(result.Value);
                                    var model = _mapper.Map<RegisterOrderForDeliveryRequestDTO>(message);
                                    var response = await _orderHandlerService.RegisterOrderForDeliveryAsync(model);
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
