using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Platform
    {
        [Key]
        public int Platform_id { get; set; }
        [Required]
        public string Description { get; set; }
        public string? ProductUrl { get; set; }
        public string? LogoUrl { get; set; }
        public string? ProductKry { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool Selling { get; set; }
        public bool Chat { get; set; }
        public bool Scrape { get; set; }
        public bool Post { get; set; }
        public bool Ads { get; set; }
        public bool Account { get; set; }
        public bool visible { get; set; } = true;

        public List<BusinessPlatforms>? BusinessPlatforms { get; set; }
    }
}
