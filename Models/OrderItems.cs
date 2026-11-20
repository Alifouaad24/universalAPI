using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class OrderItems
    {
        [Key]
        public int OrderItemsId { get; set; }

        public Item? Item { get; set; }
        [ForeignKey(nameof(Item))]
        public int ItemsId { get; set; }

        [JsonIgnore]
        public GlobalOrder? GlobalOrder { get; set; }
        [ForeignKey(nameof(GlobalOrder))]
        public int GlobalOrderId { get; set; }

        public decimal Price { get; set; }
        public Service? Service { get; set; }
        [ForeignKey(nameof(Service))]
        public int? Service_id { get; set; }


        public Size? Size { get; set; }
        [ForeignKey(nameof(Size))]
        public int Size_id { get; set; }

    }
}
