using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Business_type
    {
        [Key]
        public int Business_type_id { get; set; }

        [Required]
        public string Description { get; set; }

        public List<Business_BusinessType>? BusinessTypes { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
