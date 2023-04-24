using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary.Models.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagementService.Application.Services.Common;
using UserManagementService.Domain.Entities;

namespace UserManagementService.Infrastructure.Services
{
    //I extract this methods from AccountService. These methods were private in AccountService class.
    //I extract them here for UnitTestings purposes
    public class ItentityHelperService : IItentityHelperService
    {
        private readonly JWTConfig _jWTConfig;

        public ItentityHelperService(IOptions<JWTConfig> options)
        {
            _jWTConfig = options.Value;
        }
        public string GenerateJwtToken(User user, string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, role),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(CustomClaimTypes.UserId, user.Id.ToString()),
                new Claim(CustomClaimTypes.FirstName, user.FirstName),
                new Claim(CustomClaimTypes.LastName, user.LastName),

            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jWTConfig.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenOptions = new JwtSecurityToken
               (
                   claims: claims,
                   expires: DateTime.Now.AddDays(Convert.ToDouble(1)),
                   signingCredentials: credentials
               );

            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }
        public IEnumerable<string> GetAvailableRoles()
        {
            yield return RoleTypes.Admin;
            yield return RoleTypes.User;
            yield return RoleTypes.Courier;
        }
        public Dictionary<string, List<string>> HandleIdentityResult(IdentityResult identityResult)
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();
            foreach (var error in identityResult.Errors)
            {
                switch (error.Code)
                {
                    case "DuplicateUserName":
                        AddOrUpdateErrorDictionary(errors, "UserName", "The username is already taken.");
                        break;
                    case "DuplicateEmail":
                        AddOrUpdateErrorDictionary(errors, "Email", "The email address is already taken.");
                        break;
                    case "InvalidUserName":
                        AddOrUpdateErrorDictionary(errors, "UserName", "The username is not valid.");
                        break;
                    case "InvalidEmail":
                        AddOrUpdateErrorDictionary(errors, "Email", "The email address is not valid.");
                        break;
                    case "PasswordTooShort":
                        AddOrUpdateErrorDictionary(errors, "Password", "The password is too short. It must be at least 6 characters.");
                        break;
                    case "PasswordRequiresNonAlphanumeric":
                        AddOrUpdateErrorDictionary(errors, "Password", "The password must contain at least one non-alphanumeric character.");
                        break;
                    case "PasswordRequiresDigit":
                        AddOrUpdateErrorDictionary(errors, "Password", "The password must contain at least one digit.");
                        break;
                    case "PasswordRequiresLower":
                        AddOrUpdateErrorDictionary(errors, "Password", "The password must contain at least one lowercase letter.");
                        break;
                    case "PasswordRequiresUpper":
                        AddOrUpdateErrorDictionary(errors, "Password", "The password must contain at least one uppercase letter.");
                        break;
                    case "PasswordMismatch":
                        AddOrUpdateErrorDictionary(errors, "General", "The provided passwords do not match.");
                        break;
                    case "InvalidToken":
                        AddOrUpdateErrorDictionary(errors, "General", "The token is not valid.");
                        break;
                    case "LoginAlreadyAssociated":
                        AddOrUpdateErrorDictionary(errors, "General", "The email address is already associated with an account.");
                        break;
                    case "InvalidEmailOrPassword":
                        AddOrUpdateErrorDictionary(errors, "General", "The email address or password is incorrect.");
                        break;
                    default:
                        AddOrUpdateErrorDictionary(errors, "General", error.Description);
                        break;
                }
            }
            return errors;
        }
        public string HandleSignInResult(SignInResult result)
        {
            string errorMessage = "SignIn process failed";
            if (result.IsLockedOut)
                errorMessage = "Your account is locked";
            else if (result.IsNotAllowed)
                errorMessage = "Your are not allowed to signIn";
            else if (result.RequiresTwoFactor)
                errorMessage = "Require two factor authentication";
            return errorMessage;
        }
        //private method for handling IdentityResult
        private void AddOrUpdateErrorDictionary(Dictionary<string, List<string>> errors, string key, string message)
        {
            if (errors.ContainsKey(key))
                errors[key].Add(message);
            else
                errors.Add(key, new List<string> { message });
        }
    }
}
