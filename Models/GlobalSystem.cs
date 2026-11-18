using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class GlobalSystem
    {
        [Key]
        public int GlobalSystemId { get; set; }

        public BusinessGroup? BusinessGroup { get; set; }
        [ForeignKey(nameof(BusinessGroup))]
        public int? BusinessGroupId { get; set; }

        public string GlobalSystemName { get; set; }
        public string GlobalSystemType { get; set; }
        public string? GlobalSystemUrl { get; set; }
        [JsonIgnore]
        public List<System_Business_customer>? System_Business_customers { get; set; }
    }
}
