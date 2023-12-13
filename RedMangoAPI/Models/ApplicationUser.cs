using Microsoft.AspNetCore.Identity;

namespace RedMangoAPI
{
    public class ApplicationUser :IdentityUser
    {
        public string? Name { get; set; }
    }
}
