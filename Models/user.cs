using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Universal_server.Models
{
    public class IdentityUserData : IdentityUser
    {
        public string? UserPhoto { get; set; }
        public string? FcmToken { get; set; }
        public string? UserPassword { get; set; }
        [JsonIgnore]
        public List<UsersBusinesses>? UsersBusinesses { get; set; }

    }
}
