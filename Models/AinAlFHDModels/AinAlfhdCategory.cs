using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Universal_server.Models.AinAlFHDModels
{
    [Table("Categories")]
    public class AinAlfhdCategory
    {
        [Key]
        public int CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int? MainCategoryId { get; set; }

        public int? CategoryGroup { get; set; }
        [ForeignKey(nameof(Platform))]

        public int? Platform_id { get; set; }
        public AinAlFhdPlatform? Platform { get; set; }
        [JsonIgnore]
        public List<AinAlfhdItem>? Items { get; set; }

        [JsonIgnore]
        public List<AinAlFhdSize>? Sizes { get; set; }
    }
}
