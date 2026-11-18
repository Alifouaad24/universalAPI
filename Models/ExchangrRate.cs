using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class ExchangrRate
    {

        [Key]
        public int ExchangrRateId { get; set; }

        public Currency? CurrencySource { get; set; }
        [ForeignKey(nameof(CurrencySource))]
        public int? CurrencySourceId { get; set; }

        public Currency? CurrencyTarget { get; set; }
        [ForeignKey(nameof(CurrencyTarget))]
        public int? CurrencyTargetId { get; set; }

    }
}
