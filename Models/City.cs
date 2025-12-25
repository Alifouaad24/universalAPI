using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class City
    {
        [Key]
        public int CityId { get; set; }

        public string? Description { get; set; }

        [ForeignKey(nameof(Country))]
        public int? CountryId { get; set; }
        public Country? Country { get; set; }
        [JsonIgnore]
        public List<Area>? Areas { get; set; }

        public int? IsNorth { get; set; }
    }
}
