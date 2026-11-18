using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Color
    {
        [Key]
        public int ColorId { get; set; }
        public string Name { get; set; } = null!;
    }
}
