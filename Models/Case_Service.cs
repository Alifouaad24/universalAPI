using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Case_Service
    {
        [Key]
        public int Case_ServiceId { get; set; }

        [JsonIgnore]
        public Case? Case { get; set; }
        [ForeignKey(nameof(Case))]
        public int CaseID { get; set; }


        public Service? Service { get; set; }
        
        [ForeignKey(nameof(Service))]
        public int Service_id { get; set; }

        public bool? Resolved { get; set; }
        public string? Notes { get; set; }

        public decimal? Cost { get; set; }
        public decimal? Discount { get; set; }
        public decimal? paid { get; set; }

        public List<Case_Service_Notes>? Case_Service_Notes { get; set; }

    }
}
