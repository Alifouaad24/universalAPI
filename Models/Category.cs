using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Category
    {
        [Key]
        public int Category_id { get; set; }
        [Required]
        public string Name { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
