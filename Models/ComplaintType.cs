using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class ComplaintType
    {
        [Key]
        public int ComplaintTypeId { get; set; }
        public string Name { get; set; }
    }
}
