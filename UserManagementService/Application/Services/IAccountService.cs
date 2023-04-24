using SharedLibrary.Models.Commons;
using UserManagementService.Application.DTOs;
using UserManagementService.Application.DTOs.Request;
using UserManagementService.Application.DTOs.Response;

namespace UserManagementService.Application.Services
{
    public interface IAccountService
    {
        Task<ApiResponse<SignUpResponseDTO>> SignUpAsync(SignUpRequestDTO requestModel);
        Task<ApiResponse<SignInResponseDTO>> SignInAsync(SignInRequestDTO requestModel);
        Task<ApiResponse> CreateAccountAsync(CreateAccountRequestDTO requestModel);
        Task<ApiResponse<IEnumerable<GetAllUsersResponseDTO>>> GetAllAccountsAsync(PaginationFilterModel paginationFilterModel);
        Task<ApiResponse<IEnumerable<GetCouriersResponseDTO>>> GetCouriersAsync();
        Task<ApiResponse<GetCouriersResponseDTO>> GetCourierAsync(GetCourierRequersDTO requestModel);
    }
}
