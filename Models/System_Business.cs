using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class System_Business_customer
    {
        [Key]
        public int System_BusinessId { get; set; }

        public GlobalSystem? GlobalSystem { get; set; }
        [ForeignKey(nameof(GlobalSystem))]
        public int? GlobalSystemId { get; set; }
        [JsonIgnore]
        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }

        [JsonIgnore]
        public GlobalCustomer? GlobalCustomer { get; set; }
        [ForeignKey(nameof(GlobalCustomer))]
        public int? GlobalCustomerId { get; set; }
    }
}
