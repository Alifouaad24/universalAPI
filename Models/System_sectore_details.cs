using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class System_sectore_details
    {
        [Key]
        public int System_sectore_details_id { get; set; }
        [Required]
        public string Description { get; set; }

        /// //////////////////////////////////////////////////

        [ForeignKey(nameof(system_Sectore))]
        public int? System_sectore_id { get; set; }
        public System_sectore? system_Sectore { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
