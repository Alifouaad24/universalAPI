using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Unit
    {
        [Key]
        public int UnitId { get; set; }
        public string Name { get; set; } = null!;
        public string? NameAr { get; set; } = null!;
    }
}
