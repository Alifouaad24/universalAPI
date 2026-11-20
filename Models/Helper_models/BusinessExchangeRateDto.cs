using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models.Helper_models
{
    public class BusinessExchangeRateDto
    {
        public int? Business_id { get; set; }
        public int? SourceCurrencyId { get; set; }
        public int? TargetCurrencyId { get; set; }
        public decimal? Rate { get; set; }
    }
}
