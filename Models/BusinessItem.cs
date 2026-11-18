using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class BusinessItem
    {
        [Key]
        public int BusinessItemId { get; set; }

        [ForeignKey(nameof(Business))]
        public int? BusinessId { get; set; }
        [JsonIgnore]
        public Business? Business { get; set; }

        [ForeignKey(nameof(Item))]
        public int? ItemId { get; set; }
        [JsonIgnore]
        public Item? Item { get; set; }

        public decimal? SalePrice { get; set; }
        public int? Quantity { get; set; }

        public bool IsOnlineSaleEnabled { get; set; }
        public bool IsActive { get; set; }
    }
}
