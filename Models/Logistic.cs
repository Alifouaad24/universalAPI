using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Logistic
    {
        [Key]
        public int LogisticId { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }

        public Currency? Currency { get; set; }
        [ForeignKey(nameof(Currency))]
        public int? CurrencyId { get; set; }


        public List<Logistic_shippingType>? Logistic_shippingType { get; set; } = new List<Logistic_shippingType>();
        [JsonIgnore]
        public List<ShippingCost>? ShippingCosts { get; set; } = new List<ShippingCost>();

    }
}
