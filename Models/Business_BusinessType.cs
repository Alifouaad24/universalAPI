using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Business_BusinessType
    {
        public int Business_id { get; set; }
        [JsonIgnore]
        public Business Business { get; set; }

        public int Business_type_id { get; set; }
        [JsonIgnore]
        public Business_type BusinessType { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
