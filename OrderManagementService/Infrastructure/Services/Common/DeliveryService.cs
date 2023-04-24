using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OrderManagementService.Application.DTOs.Request;
using OrderManagementService.Application.Models.ExternalServiceModels;
using OrderManagementService.Application.Services.Common;
using OrderManagementService.Helpers.Configs;
using SharedLibrary.Helpers;
using SharedLibrary.Models.Commons;

namespace OrderManagementService.Infrastructure.Services.Common
{
    public class DeliveryService : IDeliveryService
    {
        private readonly ILogger<DeliveryService> _logger;
        private readonly HttpClient _httpClient;
        private readonly AppConfig _appConfig;

        public DeliveryService(ILogger<DeliveryService> logger,
                                HttpClient httpClient,
                                IOptions<AppConfig> appConfig)
        {
            _logger = logger;
            _httpClient = httpClient;
            _appConfig = appConfig.Value;
        }

        public async Task<IsCancelationAllowedResponseDTO> IsCancellationAllowedAsync(string orderNumber)
        {
            _logger.LogInformation($"{nameof(IsCancellationAllowedAsync)} method called");
            var requestModel = new IsCancelationAllowedRequestDTO() { OrderNumber = orderNumber };
            var urlWithQueryParam = await HttpClientHelper.GetQueryStringFromModelAsync(requestModel, _appConfig.IsCancellationAllowedEndpoint);
            var response = await _httpClient.GetAsync(urlWithQueryParam);
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Response got succesfully");
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IsCancelationAllowedResponseDTO>>(content);
                if (apiResponse.Succeeded)
                {
                    return apiResponse.Data;
                }
                else
                {
                    _logger.LogError($"Cannot get is cancelation allowed. StatusCode {response.StatusCode}");
                    throw new ApplicationException(apiResponse.ErrorMessage);
                }
            }
            else
            {
                _logger.LogError($"Cannot get is cancelation allowed. StatusCode {response.StatusCode}");
                throw new ApplicationException("Error occured while check IsCancellationAllowed method");
            }
        }
    }
}
