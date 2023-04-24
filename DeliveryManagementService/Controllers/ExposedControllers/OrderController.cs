using DeliveryManagementService.Application.DTOs.Request;
using DeliveryManagementService.Application.Filters;
using DeliveryManagementService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryManagementService.Controllers.ExposedControllers
{
    [ApiController]
    [Route("api/ex/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderHandlerService _orderHandlerService;

        public OrderController(IOrderHandlerService orderHandlerService)
        {
            _orderHandlerService = orderHandlerService;
        }

        [HttpGet("isCancelationAllowed")]
        [ValidateRequest]
        public async Task<IActionResult> IsCancelationAllowed([FromQuery] IsCancelationAllowedRequestDTO requestModel)
        {
            var result = await _orderHandlerService.IsCancelationAllowedAsync(requestModel);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
