using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Inventory
    {
        [Key]
        public int Inventory_id { get; set; }
        public string Product_name { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? Depth { get; set; }
        public string? Upc { get; set; }
        public string? Sku { get; set; }
        public string? Model { get; set; }
        public string? Notes { get; set; }

        public string? InternetId { get; set; }
        public string? Product_description { get; set; }
        public Platform? platform { get; set; }
        [ForeignKey(nameof(platform))]
        public int? Platform_id { get; set; }
        public Size? Size { get; set; }
        [ForeignKey(nameof(Size_id))]
        public int? Size_id { get; set; }
        public Category? Category { get; set; }
        [ForeignKey(nameof(Category))]
        public int? Category_id { get; set; }
        public List<Inventoy_images>? inventoy_Images { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;

    }
}
