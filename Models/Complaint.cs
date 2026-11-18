using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Complaint
    {
        [Key]
        public int ComplaintId { get; set; }
        public Complaint_Value? Complaint_Value { get; set; }

        [ForeignKey(nameof(Complaint_Value))]
        public int? Complaint_ValueId { get; set; }

        public GlobalOrder? GlobalOrder { get; set; }

        [ForeignKey(nameof(GlobalOrder))]
        public int? GlobalOrderId { get; set; }
    }
}
