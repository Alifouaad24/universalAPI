using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Universal_server.Models.AinAlFHDModels
{
    [Table("ItemImages")]
    public class AinAlfhdItemImages
    {
        [Key]
        public int ItemImagesId { get; set; }
        public string ImageLink { get; set; }

        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }
        public AinAlfhdItem Item { get; set; }

    }
}
