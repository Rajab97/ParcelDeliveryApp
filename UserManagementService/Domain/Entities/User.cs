using Microsoft.AspNetCore.Identity;
using SharedLibrary.Domain.Entities;

namespace UserManagementService.Domain.Entities
{
    public class User : IdentityUser<int>, IAuditEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
