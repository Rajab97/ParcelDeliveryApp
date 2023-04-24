using DeliveryManagementService.Application.DTOs.Request;
using DeliveryManagementService.Application.DTOs.Response;
using SharedLibrary.Models.Commons;

namespace DeliveryManagementService.Application.Services
{
    public interface IOrderHandlerService
    {
        Task<ApiResponse> RegisterOrderForDeliveryAsync(RegisterOrderForDeliveryRequestDTO requestModel);
        Task<ApiResponse<IsCancelationAllowedResponseDTO>> IsCancelationAllowedAsync(IsCancelationAllowedRequestDTO requestModel);
        Task<ApiResponse> CancelOrderAsync(CancelOrderRequestDTO requestModel);
    }
}
