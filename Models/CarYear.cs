using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class CarYear
    {
        [Key]
        public int CarYearId { get; set; }
        public string CarYearNumber { get; set; }
    }
}
