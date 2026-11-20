using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Cause
    {
        [Key]
        public int CauseId { get; set; }
        public string CauseName { get; set; }
    }
}
