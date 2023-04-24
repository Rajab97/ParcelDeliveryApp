using DeliveryManagementService.Application.Common.Models;
using DeliveryManagementService.Application.DTOs.Request;
using DeliveryManagementService.Application.DTOs.Response;
using SharedLibrary.Models.Commons;

namespace DeliveryManagementService.Application.Services
{
    public interface IDeliveryHistoryService
    {
        Task<ApiResponse> ChangeOrderStatusAsync(ChangeOrderStatusRequestDTO requestModel);
        Task<EFApiPaginationResponse<GetOrdersResponseModel>> GetOrdersAsync(PaginationFilterModel paginationFilterModel);
        Task<ApiResponse<IEnumerable<GetDeliveryHistoryOfOrderResponseDTO>>> GetDeliveryHistoryOfOrderAsync(GetDeliveryHistoryOfOrderRequestDTO requestModel);
    }
}
