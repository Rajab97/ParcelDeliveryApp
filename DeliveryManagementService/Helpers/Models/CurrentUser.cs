using SharedLibrary.Application.Common.Exceptions;
using SharedLibrary.Models.Auth;
using System.Security.Claims;

namespace DeliveryManagementService.Helpers.Models
{
    public class CurrentUser
    {
        private readonly IHttpContextAccessor _httpContext;

        public CurrentUser(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public bool IsAdmin { 
            get 
            {
                var roleClaim = _httpContext.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.Role);
                if (roleClaim == null)
                    throw new UserClaimNotMissedException();
                return roleClaim.Value == RoleTypes.Admin;
            }
        }
        public bool IsCourier
        {
            get
            {
                var roleClaim = _httpContext.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.Role);
                if (roleClaim == null)
                    throw new UserClaimNotMissedException();
                return roleClaim.Value == RoleTypes.Courier;
            }
        }
        public bool IsUser
        {
            get
            {
                var roleClaim = _httpContext.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.Role);
                if (roleClaim == null)
                    throw new UserClaimNotMissedException();
                return roleClaim.Value == RoleTypes.User;
            }
        }
        public int UserId { 
            get 
            {
                var userClaim = _httpContext.HttpContext.User.Claims.FirstOrDefault(m => m.Type == CustomClaimTypes.UserId);
                if (userClaim == null)
                    throw new UserClaimNotMissedException();
                return Convert.ToInt32(userClaim.Value);
            } 
        }

        public string? FirstName
        {
            get
            {
                var userClaim = _httpContext.HttpContext.User.Claims.FirstOrDefault(m => m.Type == CustomClaimTypes.FirstName);
                return userClaim?.Value;
            }
        }

        public string? LastName
        {
            get
            {
                var userClaim = _httpContext.HttpContext.User.Claims.FirstOrDefault(m => m.Type == CustomClaimTypes.LastName);
                return userClaim?.Value;
            }
        }

        public string Role
        {
            get
            {
                var userClaim = _httpContext.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.Role);
                if (userClaim == null)
                    throw new UserClaimNotMissedException();
                return userClaim.Value;
            }
        }

        public string? Email
        {
            get
            {
                var userClaim = _httpContext.HttpContext.User.Claims.FirstOrDefault(m => m.Type == ClaimTypes.Email);
                return userClaim?.Value;
            }
        }
    }
}
