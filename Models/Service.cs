using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Service
    {
        [Key]
        public int Service_id { get; set; }
        [Required]
        public string Description { get; set; }

        public bool IsPublic { get; set; } 
        public List<Activity_Service>? Activity_Services { get; set; }
        [JsonIgnore]
        public List<Business_Service>? Business_Services { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
