using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class caseImage
    {
        public int caseImageId { get; set; }

        [JsonIgnore]
        public GlobalOrder? Order { get; set; }
        [ForeignKey(nameof(Order))]
        public int GlobalOrderId { get; set; }
        public string ImageUrl { get; set; }
    }
}
