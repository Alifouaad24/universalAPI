using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class BusinessExchangeRate
    {

        [Key]
        public int BusinessExchangeRateId { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }

        public ExchangrRate? ExchangrRate { get; set; }
        [ForeignKey(nameof(ExchangrRate))]
        public int? ExchangrRateId { get; set; }

        public decimal? Rate { get; set; }
    }
}
