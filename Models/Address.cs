using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Address
    {
        [Key]
        public int Address_id { get; set; }

        public string Line_1 { get; set; }
        public string Line_2 { get; set; }
        public string State { get; set; }
        public string Post_code { get; set; }
        public string City { get; set; }
        [JsonIgnore]
        public List<Business_Address>? BusinessAddresses { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;


    }
}
