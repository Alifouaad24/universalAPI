using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class System_sectore
    {
        [Key]
        public int System_sectore_id { get; set; }
        [Required]
        public string Description { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
