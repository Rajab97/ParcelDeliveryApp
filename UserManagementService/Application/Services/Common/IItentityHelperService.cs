using Microsoft.AspNetCore.Identity;
using UserManagementService.Domain.Entities;

namespace UserManagementService.Application.Services.Common
{
    public interface IItentityHelperService
    {
        string GenerateJwtToken(User user, string role);
        Dictionary<string, List<string>> HandleIdentityResult(IdentityResult identityResult);
        string HandleSignInResult(SignInResult result);
        IEnumerable<string> GetAvailableRoles();
    }
}
