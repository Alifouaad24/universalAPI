using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Business_Owner
    {
        [Key]
        public int Business_OwnerId { get; set; }


        public GlobalCustomer? GlobalCustomer { get; set; }
        [ForeignKey(nameof(GlobalCustomer))]
        public int? GlobalCustomerId { get; set; }

        [JsonIgnore]
        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }
    }
}
