using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Activiity
    {
        [Key]
        public int Activity_id { get; set; }
        [Required]  
        public string Description { get; set; }
        [JsonIgnore]
        public List<Business_Activitiy>? Business_Activitiy { get; set; }
        [JsonIgnore]
        public List<Activity_Service>? Activity_Services { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
