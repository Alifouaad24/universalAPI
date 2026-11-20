using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Business_Asset
    {
        [Key]
        public int Business_AssetId { get; set; }

        public Asset? Asset { get; set; }
        [ForeignKey(nameof(Asset))]
        public int? AssetId { get; set; }
        [JsonIgnore]
        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }

        public string? Account_Id { get; set; }
    }
}
