using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Business_Activitiy
    {
        [JsonIgnore] 
        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int Business_id { get; set; }

        public Activiity? Activiity { get; set; }
        [ForeignKey(nameof(Activiity))]
        public int Activity_id { get; set; }
    }
}
