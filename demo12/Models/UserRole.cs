using Microsoft.AspNetCore.Identity;

namespace demo12.Models
{
    public class UserRole:IdentityUserRole<string>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}
