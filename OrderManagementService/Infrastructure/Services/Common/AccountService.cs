using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderManagementService.Application.Models.ExternalServiceModels;
using OrderManagementService.Application.Services.Common;
using OrderManagementService.Helpers;
using OrderManagementService.Helpers.Configs;
using SharedLibrary.Helpers;
using SharedLibrary.Models.Commons;

namespace OrderManagementService.Infrastructure.Services.Common
{
    public class AccountService : IAccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly HttpClient _httpClient;
        private readonly AppConfig _appConfig;

        public AccountService(ILogger<AccountService> logger,
                                HttpClient httpClient,
                                IOptions<AppConfig> appConfig)
        {
            _logger = logger;
            _httpClient = httpClient;
            _appConfig = appConfig.Value;
        }

        public async Task<IEnumerable<GetCouriersResponseDTO>> GetCouriersAsync()
        {
            _logger.LogInformation($"{nameof(GetCouriersAsync)} method called");
            var response = await _httpClient.GetAsync(_appConfig.GetCouiriersEndpoint);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Courier list getted succesfully");
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<GetCouriersResponseDTO>>>(content);
                if (apiResponse.Succeeded)
                    return apiResponse.Data;
                else
                    throw new ApplicationException($"{apiResponse.ErrorMessage}");
            }
            else
            {
                _logger.LogError($"Cannot get courier list. StatusCode {response.StatusCode}");
                throw new ApplicationException("Error occured while get couriers");
            }
        }

        public async Task<GetCouriersResponseDTO> GetCourierAsync(int userId)
        {
            _logger.LogInformation($"{nameof(GetCourierAsync)} method called");
            var requestModel = new GetCourierRequersDTO() { UserId = userId };
            var urlWithQueryParam = await HttpClientHelper.GetQueryStringFromModelAsync(requestModel, _appConfig.GetCourierEndpoint);
            var response = await _httpClient.GetAsync(urlWithQueryParam);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Courier getted succesfully");
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<GetCouriersResponseDTO>>(content);
                if (apiResponse.Succeeded)
                    return apiResponse.Data;
                else
                    throw new ApplicationException($"{apiResponse.ErrorMessage}");
            }
            else
            {
                _logger.LogError($"Cannot check is user courier. StatusCode {response.StatusCode}");
                throw new ApplicationException("Error occured while check IsCourier method");
            }
        }
    }
}
