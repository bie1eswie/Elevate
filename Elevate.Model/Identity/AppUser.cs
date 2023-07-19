using Microsoft.AspNetCore.Identity;

namespace Elevate.Model.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
    }
}
