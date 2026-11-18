
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class BusinessPlatformCountries
    {
        [Key]
        public int BusinessPlatformCountriesId { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int BusinessId { get; set; }

        public Country? Country { get; set; }
        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }

        public Platform? Platform { get; set; }
        [ForeignKey(nameof(Platform))]
        public int? PlatformId { get; set; }

        public ShippingType? ShippingType { get; set; }
        [ForeignKey(nameof(ShippingType))]
        public int? ShippingTypeId { get; set; }

        public Taaref? Tariff { get; set; }
        [ForeignKey(nameof(Tariff))]
        public int? TaarefId { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
