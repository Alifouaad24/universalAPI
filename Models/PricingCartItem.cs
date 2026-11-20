using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class PricingCartItem
    {
        [Key]
        public int PricingCartItemId { get; set; }

        public PricingCart? PricingCart { get; set; }
        [ForeignKey(nameof(PricingCart))]
        public int? PricingCartId { get; set; }

        public Item? Item { get; set; }
        [ForeignKey(nameof(Item))]
        public int? ItemId { get; set; }
    }
}
