using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Buseness_Customer
    {
        [Key]
        public int Buseness_CustomerId { get; set; }

        public bool IsOwner { get; set; } = false;

        [JsonIgnore]
        public GlobalCustomer? GlobalCustomer { get; set; }
        [ForeignKey(nameof(GlobalCustomer))]
        public int? GlobalCustomerId { get; set; }

        [JsonIgnore]
        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }
    }
}
