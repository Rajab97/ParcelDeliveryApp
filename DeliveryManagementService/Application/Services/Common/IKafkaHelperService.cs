namespace DeliveryManagementService.Application.Services.Common
{
    public interface IKafkaHelperService
    {
        Task ProduceMessageAsync<T>(T message, string topic);
        Task ProduceMessageWithKeysync<TKey, TMessage>(TKey key, TMessage message, string topic) where TKey : struct;
    }
}
