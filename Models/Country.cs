using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }
        [Required]
        public string Name { get; set; }
        public List<Business>? Businesses { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;

    }
}
