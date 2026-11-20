using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Serving
    {
        [Key]
        public int ServingId { get; set; }
        public string? ServingName { get; set; }
    }
}
