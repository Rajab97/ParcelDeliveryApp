using OrderManagementService.Application.Models.ExternalServiceModels;

namespace OrderManagementService.Application.Services.Common
{
    public interface IAccountService
    {
        Task<IEnumerable<GetCouriersResponseDTO>> GetCouriersAsync();
        Task<GetCouriersResponseDTO> GetCourierAsync(int userId);
    }
}
