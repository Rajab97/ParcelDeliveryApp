using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using DeliveryManagementService.Helpers.Configs;
using SharedLibrary.Models.KafkaSchemaRegistry;
using System.Text;
using DeliveryManagementService.Application.Services.Common;

namespace DeliveryManagementService.Infrastructure.Services.Common
{
    public class KafkaHelperService : IKafkaHelperService
    {
        private readonly ILogger<KafkaHelperService> _logger;
        private readonly KafkaConfig _kafkaConfig;

        public KafkaHelperService(ILogger<KafkaHelperService> logger,
                                    IOptions<KafkaConfig> kafkaConfig)
        {
            _logger = logger;
            _kafkaConfig = kafkaConfig.Value;
        }

        public async Task ProduceMessageAsync<T>(T message,string topic)
        {
            _logger.LogInformation($"Start produce message to \"{topic}\" topic. Message: {JsonConvert.SerializeObject(message,new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaConfig.Host,
                // Other configuration properties as needed
            };
         
            var builder = new ProducerBuilder<Null, string>(config);
            using (var producer = builder.Build())
            {
                var kafkaMessage = new Message<Null, string>
                {
                    Value = JsonConvert.SerializeObject(message)
                };
                var result = await producer.ProduceAsync(topic, kafkaMessage);
                if (result.Status == PersistenceStatus.Persisted)
                {
                    _logger.LogInformation($"Message delivered to partition {result.Partition} at offset {result.Offset}");
                }
                else
                {
                    _logger.LogError($"Message delivery failed: {result.Message}");
                    throw new KafkaProducerException();
                }
            }
        }

        public async Task ProduceMessageWithKeysync<TKey,TMessage>(TKey key,TMessage message, string topic) where TKey : struct
        {
            _logger.LogInformation($"Start produce message to \"{topic}\" topic. Message: {JsonConvert.SerializeObject(message, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore })}");
            var config = new ProducerConfig
            {
                BootstrapServers = _kafkaConfig.Host,
                // Other configuration properties as needed
            };
            var builder = new ProducerBuilder<TKey, string>(config);
            using (var producer = builder.Build())
            {
                var kafkaMessage = new Message<TKey, string>
                {
                    Key = key,
                    Value = JsonConvert.SerializeObject(message)
                };
                var result = await producer.ProduceAsync(topic, kafkaMessage);
                if (result.Status == PersistenceStatus.Persisted)
                {
                    _logger.LogInformation($"Message delivered to partition {result.Partition} at offset {result.Offset}");
                }
                else
                {
                    _logger.LogError($"Message delivery failed: {result.Message}");
                    throw new KafkaProducerException();
                }
            }
        }
    }
}
