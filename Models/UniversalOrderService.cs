using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class UniversalOrderService
    {
        [Key]
        public int OrderServiceId { get; set; }
        public Service? Service { get; set; }
        [ForeignKey(nameof(Service))]
        public int? Service_id { get; set; }

        public UniversalOrder UniversalOrder { get; set; }
        [ForeignKey(nameof(UniversalOrder))]
        public int? UniversalOrderId { get; set; }

        public Item? Item { get; set; }
        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        public decimal Price { get; set; }


        public Size? Size { get; set; }
        [ForeignKey(nameof(Size))]
        public int Size_id { get; set; }

    }
}
