using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Inventory
    {
        [Key]
        public int Inventory_id { get; set; }
        public string? Product_name { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? Depth { get; set; }
        public string? Upc { get; set; }
        public string? Sku { get; set; }
        public string? Model { get; set; }
        public string? Notes { get; set; }
        public string? SitePrice { get; set; }
        public int? FolderImages { get; set; } // to show all images were picked by app
        public int? QtyInInventory { get; set; }
        public int? QtyPublished { get; set; }
        public string? InternetId { get; set; }
        public string? Product_description { get; set; }
        public Platform? platform { get; set; }
        [ForeignKey(nameof(platform))]
        public int? Platform_id { get; set; }
        public Size? Size { get; set; }
        [ForeignKey(nameof(Size))]
        public int? Size_id { get; set; }
        public Category? Category { get; set; }
        [ForeignKey(nameof(Category))]
        public int? Category_id { get; set; }

        public Item? Item { get; set; }
        [ForeignKey(nameof(Item))]
        public int? ItemId { get; set; }

        public ItemCondition? ItemCondition { get; set; }
        [ForeignKey(nameof(ItemCondition))]
        public int? ItemConditionId { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }
        public List<Inventoy_images>? inventoy_Images { get; set; }

        public DateTime? Insert_on { get; set; }
        public string? EbayInvID { get; set; }
        public string? EbayOfferID { get; set; }
        public string? Status { get; set; }
        public string? OldStatus { get; set; }
        public string? EbayListingId { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
        public bool IsSold { get; set; } = false;
        public bool IsPublishedOnMarketPlace { get; set; } = false;
        public string? MarketPlaceOfferUrl { get; set; }
        public bool IsProccessedInInventory { get; set; } = false;
        public bool NotFound { get; set; } = false;

    }
}
