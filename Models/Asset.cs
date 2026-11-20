using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Asset
    {
        [Key]
        public int AssetId { get; set; }
        public string AssetName { get; set; }
        public string AssetType { get; set; }

        [JsonIgnore]
        public List<Business_Asset>? Business_Assets { get; set; }
    }
}
