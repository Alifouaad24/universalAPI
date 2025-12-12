using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Activiity
    {
        [Key]
        public int Activity_id { get; set; }
        [Required]  
        public string Description { get; set; }

        [ForeignKey(nameof(business))]
        public int Business_id { get; set; }
        public Business? business { get; set; }
        public List<Service>? Services { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
