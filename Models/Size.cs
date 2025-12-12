using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Size
    {
        [Key]
        public int Size_id { get; set; }
        [Required]
        public string Description { get; set; }

        [ForeignKey(nameof(Category))]
        public int? Category_id { get; set; }
        public Category? Category { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
