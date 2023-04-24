using OrderManagementService.Application.Models.ExternalServiceModels;

namespace OrderManagementService.Application.Services.Common
{
    public interface IDeliveryService
    {
        Task<IsCancelationAllowedResponseDTO> IsCancellationAllowedAsync(string orderNumber);
    }
}
