using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class BusinessGroupBusinesses
    {
        [Key]
        public int BusinessGroupBusinessesId { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int Business_id { get; set; }

        [JsonIgnore]
        public BusinessGroup? BusinessGroup { get; set; }
        [ForeignKey(nameof(BusinessGroup))]
        public int BusinessGroupsId { get; set; }
    }
}
