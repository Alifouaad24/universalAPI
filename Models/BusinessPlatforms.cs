using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class BusinessPlatforms
    {
        [Key]
        public int BusinessPlatformsId { get; set; }

        [JsonIgnore]
        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }

        [JsonIgnore]
        public Platform? Platform { get; set; }
        [ForeignKey(nameof(Platform))]
        public int? Platform_id { get; set; }

        public bool? ForScraping { get; set; } = true;
        public bool? ForSelling { get; set; } = true;
    }
}
