using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class ItemImage
    {
        [Key]
        public int ItemImageId { get; set; }

        [ForeignKey(nameof(Item))]
        public int? ItemId { get; set; }
        [JsonIgnore]
        public Item? Item { get; set; }
        public string ImageUrl { get; set; }
        public string? UserId { get; set; }
        public bool IsMain { get; set; } = false;
        public bool? IsPublishedOnEbay { get; set; } = false;
    }
}
