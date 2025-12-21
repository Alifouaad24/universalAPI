using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Business_Service
    {
        [ForeignKey(nameof(Business))]
        public int Business_id { get; set; }
        [JsonIgnore]
        public Business? Business { get; set; }

        [ForeignKey(nameof(Service))]
        public int Service_id { get; set; }
        
        public Service? Service { get; set; }

    }
}
