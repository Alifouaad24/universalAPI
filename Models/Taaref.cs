using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Taaref
    {
        [Key]
        public int TaarefId { get; set; }
        public Platform? Platform { get; set; }
        [ForeignKey(nameof(Platform))]
        public int? Platform_id { get; set; }
        public int? TariffType { get; set; }
        public bool IsActive { get; set; } = true ;
        public List<TariffDetails>? TariffDetails { get; set; }

    }
}
