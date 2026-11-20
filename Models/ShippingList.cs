using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class ShippingList
    {
        [Key]
        public int ShippingListId { get; set; }

        public string? NackName { get; set; }
        public Country? FromCountry { get; set; }
        [ForeignKey(nameof(FromCountry))]
        public int? FromCountryId { get; set; }

        public Country? ToCountry { get; set; }
        [ForeignKey(nameof(ToCountry))]
        public int? ToCountryId { get; set; }

        public ShippingType? ShippingType { get; set; }
        [ForeignKey(nameof(ShippingType))]
        public int? ShippingTypeId { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }


        public bool IsActive { get; set; } = true;
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
