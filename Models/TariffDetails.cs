using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class TariffDetails
    {
        public int TariffDetailsId { get; set; }

        [JsonIgnore]
        public Taaref? Taaref { get; set; }
        [ForeignKey(nameof(Taaref))]
        public int? TaarefId { get; set; }

        public Service? Service { get; set; }
        [ForeignKey(nameof(Service))]
        public int? Service_id { get; set; }

        public ShippingList? ShippingList { get; set; }
        [ForeignKey(nameof(ShippingList))]
        public int? ShippingListId { get; set; }


        public Currency? Currency { get; set; }
        [ForeignKey(nameof(Currency))]
        public int? CurrencyId { get; set; }


        public Unit? Unit { get; set; }
        [ForeignKey(nameof(Unit))]
        public int? UnitId { get; set; }

        public decimal Price { get; set; }
        
        
        public bool IsActive { get; set; } = true;

    }
}
