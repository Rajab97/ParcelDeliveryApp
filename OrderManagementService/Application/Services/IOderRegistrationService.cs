using OrderManagementService.Application.Common.Models;
using OrderManagementService.Application.DTOs.Request;
using OrderManagementService.Application.DTOs.Response;
using SharedLibrary.Models.Commons;

namespace OrderManagementService.Application.Services
{
    public interface IOderRegistrationService
    {
        Task<ApiResponse<CreateOrderResponseDTO>> CreateOrderAsync(CreateOrderRequestDTO requestDTO);
        Task<ApiResponse> ChangeOrderAddressesAsync(ChangeOrderAddressRequestDTO requestModel);
        Task<ApiResponse> AssignToCurierAsync(AssignToCurierRequestDTO requestModel);
        Task<ApiResponse> ChangeOrderStatusAsync(ChangeOrderStatusRequestDTO requestModel);
        Task<ApiResponse> CancelOrderAsync(CancelOrderRequestDTO requestModel);
        Task<EFApiPaginationResponse<GetOrdersResponseDTO>> GetOrdersAsync(PaginationFilterModel paginationFilter);
    }
}
