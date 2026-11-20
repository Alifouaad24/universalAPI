using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Complaint_Value
    {

        [Key]
        public int Complaint_ValueId { get; set; }

        public ComplaintType? ComplaintType { get; set; }

        [ForeignKey(nameof(ComplaintType))]
        public int? ComplaintTypeId { get; set; }
        public string? Value { get; set; }
        public string? Description { get; set; }

    }
}
