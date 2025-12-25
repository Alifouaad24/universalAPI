using Microsoft.AspNetCore.Identity;

namespace Universal_server.Models
{
    public class IdentityUserData : IdentityUser
    {
        public string? UserPhoto { get; set; }
        public string? UserPassword { get; set; }
        public List<UsersBusinesses>? UsersBusinesses { get; set; }

    }
}
