using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLibrary.Models.Auth;
using SharedLibrary.Models.Commons;
using Swashbuckle.AspNetCore.Annotations;
using UserManagementService.Application.DTOs;
using UserManagementService.Application.DTOs.Response;
using UserManagementService.Application.Filters;
using UserManagementService.Application.Services;
using UserManagementService.Domain.Entities;

namespace UserManagementService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }
        [HttpPost("signin")]
        [AllowAnonymous]
        [ValidateRequest]
        [SwaggerOperation(Summary = "Sign method for getting JWT token",
                            Description = "This method gets a username and password and retrive JWT token with your claims for use in other services.")]
        [SwaggerResponse(StatusCodes.Status200OK,Description = "User successfully signed")]
        [SwaggerResponse(StatusCodes.Status400BadRequest,Description = "User credentials aren't correct")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError,Description = "Internal error occured")]
        public async Task<IActionResult> SignIn([FromBody] SignInRequestDTO requestModel)
        {
            var result = await _accountService.SignInAsync(requestModel);

            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("signup")]
        [AllowAnonymous]
        [ValidateRequest]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestDTO requestModel)
        {
            var result = await _accountService.SignUpAsync(requestModel);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpPost("createAccount")]
        [Authorize(Roles = RoleTypes.Admin)]
        [ValidateRequest]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccountRequestDTO requestModel)
        {
            var result = await _accountService.CreateAccountAsync(requestModel);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("getAccounts")]
        [Authorize(Roles = RoleTypes.Admin)]
        public async Task<IActionResult> GetAccounts([FromQuery] PaginationFilterModel requestModel)
        {
            var result = await _accountService.GetAllAccountsAsync(requestModel);
            if (result.Succeeded)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
