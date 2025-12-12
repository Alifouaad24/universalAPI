using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Inventoy_images
    {
        [Key]
        public int Inventoy_images_id { get; set; }
        [Required]
        public string Inventoy_images_url { get; set; }

        [ForeignKey(nameof(Inventory))]
        public int Inventory_id { get; set; }
        public Inventory? Inventory { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
