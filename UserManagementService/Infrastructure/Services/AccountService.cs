using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Serilog;
using SharedLibrary.Application.Common.Exceptions;
using SharedLibrary.Domain.Repositories;
using SharedLibrary.Models.Auth;
using SharedLibrary.Models.Commons;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementService.Application.Common.Models;
using UserManagementService.Application.DTOs;
using UserManagementService.Application.DTOs.Request;
using UserManagementService.Application.DTOs.Response;
using UserManagementService.Application.Services;
using UserManagementService.Application.Services.Common;
using UserManagementService.Domain.Entities;
using UserManagementService.Helpers.Configs;

namespace UserManagementService.Infrastructure.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly ILogger<AccountService> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly IItentityHelperService _itentityHelperService;

        public AccountService(UserManager<User> userManager,
                                SignInManager<User> signInManager,
                                            IMapper mapper,
                                                IItentityHelperService itentityHelperService,
                                                    RoleManager<IdentityRole<int>> roleManager,
                                                        ILogger<AccountService> logger
                                       )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _signInManager = signInManager;
            _mapper = mapper;
            _itentityHelperService = itentityHelperService;
        }
        public async Task<ApiResponse<IEnumerable<GetCouriersResponseDTO>>> GetCouriersAsync()
        {
            _logger.LogInformation($"{nameof(GetCouriersAsync)} method called");
            var couriers = await _userManager.GetUsersInRoleAsync(RoleTypes.Courier);
            _logger.LogInformation($"{couriers.Count} rows returned");
            var result = couriers.Select(m => new GetCouriersResponseDTO()
            {
                Id = m.Id,
                Email = m.Email,
                FirstName = m.FirstName,
                LastName = m.LastName,
                UserName = m.UserName
            });
            return ApiResponse.Success(result);
        }
        public async Task<ApiResponse<GetCouriersResponseDTO>> GetCourierAsync(GetCourierRequersDTO requestModel)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(m=> m.Id == requestModel.UserId);
            if (user == null)
                throw new NotFoundException();

            var result = await _userManager.IsInRoleAsync(user,RoleTypes.Courier);
            if (result)
            {
                var userDTO = _mapper.Map<GetCouriersResponseDTO>(user);
                return ApiResponse.Success(userDTO);
            }
            return ApiResponse<GetCouriersResponseDTO>.Error($"User isn't in {RoleTypes.Courier} role") ;
        } 
        public async Task<ApiResponse<IEnumerable<GetAllUsersResponseDTO>>> GetAllAccountsAsync(PaginationFilterModel paginationFilterModel)
        {
            var users = await _userManager.Users
                                        .Skip((paginationFilterModel.PageIndex - 1) * paginationFilterModel.PageSize)
                                            .Take(paginationFilterModel.PageSize)
                                                .ToListAsync();

            var usersWithRoles = new List<GetAllUsersResponseDTO>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                usersWithRoles.Add(new GetAllUsersResponseDTO
                {
                    Id = user.Id,
                    Created = user.CreatedAt,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.FirstName,
                    Role = roles.FirstOrDefault()
                });
            }

            return ApiResponse<IEnumerable<GetAllUsersResponseDTO>>.Success(usersWithRoles);
        }
        public async Task<ApiResponse<SignUpResponseDTO>> SignUpAsync(SignUpRequestDTO requestModel)
        {
            Log.Information($"SignUp method called");

           
            var user = _mapper.Map<User>(requestModel);
            user.CreatedAt = DateTime.Now;
            Log.Information($"Mapped user data {JsonConvert.SerializeObject(user)}");
            var identityResult = await _userManager.CreateAsync(user,requestModel.Password);
            Log.Information($"User create response is {JsonConvert.SerializeObject(identityResult)}");
            if (identityResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user,RoleTypes.User);
                Log.Information($"Role add response is {JsonConvert.SerializeObject(roleResult)}");
                //Check if role added than return JWT to user else remove user and return RoleResult errors
                if (roleResult.Succeeded)
                {
                    var createdUser = await _userManager.FindByEmailAsync(requestModel.Email);
                    var roles = await _userManager.GetRolesAsync(createdUser);
                    var jwtToken = _itentityHelperService.GenerateJwtToken(createdUser, roles.FirstOrDefault());
                    return ApiResponse.Success(new SignUpResponseDTO() { Token = jwtToken });
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                    var errors = _itentityHelperService.HandleIdentityResult(roleResult);
                    return ApiResponse<SignUpResponseDTO>.Error(errors.ToDictionary(m => m.Key, m => m.Value as IEnumerable<string>));
                }
            }
            else
            {
                var errors = _itentityHelperService.HandleIdentityResult(identityResult);
                return ApiResponse<SignUpResponseDTO>.Error(errors.ToDictionary(m=>m.Key,m=> m.Value as IEnumerable<string>));
            }
        }
        public async Task<ApiResponse> CreateAccountAsync(CreateAccountRequestDTO requestModel)
        {
            Log.Information($"CreateAccountAsync method called");


            if (!await _roleManager.RoleExistsAsync(requestModel.Role))
                throw new ApplicationException("Role doesn't exist");

            var user = _mapper.Map<User>(requestModel);
            user.CreatedAt = DateTime.Now;
            Log.Information($"Mapped user data {JsonConvert.SerializeObject(user)}");
            var identityResult = await _userManager.CreateAsync(user, requestModel.Password);
            Log.Information($"User create response is {JsonConvert.SerializeObject(identityResult)}");
            if (identityResult.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, requestModel.Role);
                Log.Information($"Role add response is {JsonConvert.SerializeObject(roleResult)}");
                //Check if role added than return JWT to user else remove user and return RoleResult errors
                if (roleResult.Succeeded)
                {
                    var createdUser = await _userManager.FindByEmailAsync(requestModel.Email);
                    var roles = await _userManager.GetRolesAsync(createdUser);
                    var jwtToken = _itentityHelperService.GenerateJwtToken(createdUser, roles.FirstOrDefault());
                    return ApiResponse.Success();
                }
                else
                {
                    await _userManager.DeleteAsync(user);
                    var errors = _itentityHelperService.HandleIdentityResult(roleResult);
                    return ApiResponse.Error(errors.ToDictionary(m => m.Key, m => m.Value as IEnumerable<string>));
                }
            }
            else
            {
                var errors = _itentityHelperService.HandleIdentityResult(identityResult);
                return ApiResponse.Error(errors.ToDictionary(m => m.Key, m => m.Value as IEnumerable<string>));
            }
        }
        public async Task<ApiResponse<SignInResponseDTO>> SignInAsync(SignInRequestDTO requestModel)
        {
            Log.Information("SignIn method called");
            var result = await _signInManager.PasswordSignInAsync(requestModel.UserName, requestModel.Password, isPersistent: false, lockoutOnFailure: false);
            Log.Information($"PasswordSignIn response is {JsonConvert.SerializeObject(result)}");
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(requestModel.UserName);
                var roles = await _userManager.GetRolesAsync(user);
                var jwtToken = _itentityHelperService.GenerateJwtToken(user, roles.FirstOrDefault());

                return ApiResponse.Success(new SignInResponseDTO() { Token = jwtToken });
            }
            else
            {
                var errorMessage = _itentityHelperService.HandleSignInResult(result);
                return ApiResponse<SignInResponseDTO>.Error(errorMessage);
            }
        }
    }
}
