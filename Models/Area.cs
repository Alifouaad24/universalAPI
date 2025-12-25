using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Area
    {
        [Key]
        public int AreaId { get; set; }

        public string? Description { get; set; }

        public City? City { get; set; }

        [ForeignKey(nameof(City))]
        public int? CityId { get; set; }

        public int? Sector { get; set; }

        public int? Zone { get; set; }

        public int? Spec { get; set; }
    }
}
