namespace OrderManagementService.Application.Services.Common
{
    public interface IKafkaHelperService
    {
        Task ProduceMessageAsync(string message, string topic);
        Task ProduceMessageWithKeysync<TKey>(TKey key, string message, string topic) where TKey : struct;
    }
}
