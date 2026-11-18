using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Case_Service_Notes
    {
        [Key]
        public int Case_ServiceNotesId { get; set; }
        public string? Notes { get; set; }
        [JsonIgnore]
        public GlobalOrderDetail? GlobalOrderDetail { get; set; }

        [ForeignKey(nameof(GlobalOrderDetail))]
        public int? GlobalOrderDetailId { get; set; }
    }
}
