using Swashbuckle.AspNetCore.Annotations;

namespace UserManagementService.Application.DTOs
{
    public class SignInRequestDTO : IValidatableDTO
    {
        [SwaggerSchema("Your account username")]
        public string UserName { get; set; }

        [SwaggerSchema("Your account password")]
        public string Password { get; set; }
    }
}
