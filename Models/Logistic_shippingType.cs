using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Logistic_shippingType
    {
        [Key]
        public int Logistic_shippingTypeId { get; set; }
        [JsonIgnore]
        public Logistic? Logistic { get; set; }
        [ForeignKey(nameof(Logistic))]
        public int? LogisticId { get; set; }
        [JsonIgnore]
        public ShippingType? ShippingType { get; set; }
        [ForeignKey(nameof(ShippingType))]
        public int? ShippingTypeId { get; set; }
    }
}
