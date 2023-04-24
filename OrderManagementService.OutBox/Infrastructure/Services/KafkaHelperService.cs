using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderManagementService.Application.Services.Common;
using OrderManagementService.Helpers.Configs;
using SharedLibrary.Application.Common.Exceptions;
using SharedLibrary.Models.KafkaSchemaRegistry;
using System.Text;

namespace OrderManagementService.Infrastructure.Services.Common
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

        public async Task ProduceMessageAsync(string message,string topic)
        {
            _logger.LogInformation($"Start produce message to \"{topic}\" topic. Message: {message}");
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
                    Value = message
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

        public async Task ProduceMessageWithKeysync<TKey>(TKey key,string message, string topic) where TKey : struct
        {
            _logger.LogInformation($"Start produce message to \"{topic}\" topic. Message: {message}");
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
                    Value = message
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
