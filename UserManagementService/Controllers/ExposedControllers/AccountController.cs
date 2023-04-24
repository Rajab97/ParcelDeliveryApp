using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.Auth;
using SharedLibrary.Models.Commons;
using System.Data;
using UserManagementService.Application.DTOs.Request;
using UserManagementService.Application.Services;

namespace UserManagementService.Controllers.ExposedControllers
{
    [ApiController]
    [Route("api/ex/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("getCouriers")]
        public async Task<IActionResult> GetCouriers()
        {
            var result = await _accountService.GetCouriersAsync();
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getCourier")]
        public async Task<IActionResult> GetCourier([FromQuery] GetCourierRequersDTO requestModel)
        {
            var result = await _accountService.GetCourierAsync(requestModel);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
