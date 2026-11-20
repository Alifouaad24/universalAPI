using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Assign_type
    {
        [Key]
        public int Assign_typeId { get; set; }
        public string Type { get; set; }
    }
}
