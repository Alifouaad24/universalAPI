using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class UsersBusinesses
    {
        [ForeignKey(nameof(UserData))]
        public string UserId { get; set; }
        [JsonIgnore]
        public IdentityUserData? UserData { get; set; }


        [ForeignKey(nameof(Business))]
        public int Business_id {  get; set; }
        [JsonIgnore]
        public Business? Business { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
