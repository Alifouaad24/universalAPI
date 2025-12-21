using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Activity_Service
    {
        [ForeignKey(nameof(Activiity))]
        public int Activity_id { get; set; }
        [JsonIgnore]
        public Activiity? Activiity { get; set; }

        [ForeignKey(nameof(Service))]
        public int Service_id { get; set; }
        [JsonIgnore]
        public Service? Service { get; set; }
    }
}
