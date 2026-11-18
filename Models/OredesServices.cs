using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class OredesServices
    {
        [Key]
        public int OredesServicesId { get; set; }


        public Service? Service { get; set; }
        [ForeignKey(nameof(Service))]
        public int? ServiceId { get; set; }

        [JsonIgnore]
        public GlobalOrder? GlobalOrder { get; set; }
        [ForeignKey(nameof(GlobalOrder))]
        public int? GlobalOrderId { get; set; }

        public CarInfoTbl? CarInfoTbl { get; set; }
        [ForeignKey(nameof(CarInfoTbl))]
        public int? CarInfoId { get; set; }


        public bool? Resolved { get; set; }
        public string? Notes { get; set; }

        public decimal? Cost { get; set; }
        public decimal? Discount { get; set; }
        public decimal? paid { get; set; }

        public List<Case_Service_Notes>? Case_Service_Notes { get; set; }

    }
}
