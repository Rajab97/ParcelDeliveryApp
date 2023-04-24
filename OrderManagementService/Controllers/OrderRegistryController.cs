using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementService.Application.Common.Constants;
using OrderManagementService.Application.DTOs.Request;
using OrderManagementService.Application.Filters;
using OrderManagementService.Application.Services;
using SharedLibrary.Models.Auth;
using SharedLibrary.Models.Commons;

namespace OrderManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderRegistryController : ControllerBase
    {
      
        private readonly IOderRegistrationService _oderRegistrationService;

        public OrderRegistryController(IOderRegistrationService oderRegistrationService)
        {
            _oderRegistrationService = oderRegistrationService;
        }

        [HttpGet("getOrders")]
        [Authorize(Policy = CustomPolicyTypes.UserPolicy)]
        public async Task<IActionResult> GetOrders([FromQuery]PaginationFilterModel requestModel)
        {
            var result = await _oderRegistrationService.GetOrdersAsync(requestModel);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("createOrder")]
        [Authorize(Policy = CustomPolicyTypes.UserPolicy)]
        [ValidateRequest]
        public async Task<IActionResult> CreateOrder(CreateOrderRequestDTO requestModel)
        {
            var result = await _oderRegistrationService.CreateOrderAsync(requestModel);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("assignToCurier")]
        [Authorize(Policy = CustomPolicyTypes.OnlyAdminPolicy)]
        [ValidateRequest]
        public async Task<IActionResult> AssignToCurier(AssignToCurierRequestDTO requestModel)
        {
            var result = await _oderRegistrationService.AssignToCurierAsync(requestModel);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("cancelOrder")]
        [Authorize(Policy = CustomPolicyTypes.UserPolicy)]
        [ValidateRequest]
        public async Task<IActionResult> CancelOrder(CancelOrderRequestDTO requestModel)
        {
            var result = await _oderRegistrationService.CancelOrderAsync(requestModel);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPatch("changeOrderAddresses")]
        [Authorize(Policy = CustomPolicyTypes.UserPolicy)]
        [ValidateRequest]
        public async Task<IActionResult> ChangeOrderAddresses(ChangeOrderAddressRequestDTO requestModel)
        {
            var result = await _oderRegistrationService.ChangeOrderAddressesAsync(requestModel);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
    }
}