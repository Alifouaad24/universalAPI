using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Service
    {
        [Key]
        public int Service_id { get; set; }
        [Required]
        public string Description { get; set; }

        [ForeignKey(nameof(Activiity))]
        public int? Activity_id { get; set; }
        public Activiity? Activiity { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
