using Microsoft.AspNetCore.Identity;

namespace demo12.Models
{
    public class ApplicationRole : IdentityRole
    {

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
