using Microsoft.AspNetCore.Identity;

namespace IdentityAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
