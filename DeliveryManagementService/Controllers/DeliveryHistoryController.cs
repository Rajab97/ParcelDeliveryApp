using DeliveryManagementService.Application.DTOs.Request;
using DeliveryManagementService.Application.Filters;
using DeliveryManagementService.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.Commons;

namespace DeliveryManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryHistoryController : ControllerBase
    {
        private readonly IDeliveryHistoryService _deliveryHistoryService;

        public DeliveryHistoryController(IDeliveryHistoryService deliveryHistoryService)
        {
            _deliveryHistoryService = deliveryHistoryService;
        }

        [HttpPost("changeOrderStatus")]
        [Authorize(Policy = CustomPolicyTypes.CourierPolicy)]
        [ValidateRequest]
        public async Task<IActionResult> ChangeOrderStatus(ChangeOrderStatusRequestDTO requestDTO)
        {
            var result = await _deliveryHistoryService.ChangeOrderStatusAsync(requestDTO);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getOrders")]
        [Authorize(Policy = CustomPolicyTypes.CourierPolicy)]
        public async Task<IActionResult> GetOrders([FromQuery]PaginationFilterModel requestDTO)
        {
            var result = await _deliveryHistoryService.GetOrdersAsync(requestDTO);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("getOrderDeliveryHistory")]
        [Authorize(Policy = CustomPolicyTypes.AllUsersPolicy)]
        [ValidateRequest]
        public async Task<IActionResult> GetOrderDeliveryHistory([FromQuery] GetDeliveryHistoryOfOrderRequestDTO requestDTO)
        {
            var result = await _deliveryHistoryService.GetDeliveryHistoryOfOrderAsync(requestDTO);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
