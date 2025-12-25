using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Address
    {
        [Key]
        public int Address_id { get; set; }

        public string? Line_1 { get; set; }
        public string? Line_2 { get; set; }

        [ForeignKey(nameof(State))]
        public int? StateId { get; set; }
        public State? State { get; set; }

        public string? Post_code { get; set; }
        public string? Land_Mark { get; set; }

        [ForeignKey(nameof(City))]
        public int? CityId { get; set; }
        public City? City { get; set; }

        [ForeignKey(nameof(Area))]
        public int? AreaId { get; set; }
        public Area? Area { get; set; }


        [ForeignKey(nameof(Country))]
        public int? CountryId { get; set; }
        public Country? Country { get; set; }

        [JsonIgnore]
        public List<Business_Address>? BusinessAddresses { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;


    }
}
