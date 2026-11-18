using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class ShippingType
    {
        [Key]
        public int ShippingTypeId {  get; set; }
        public string Name {  get; set; }
        public List<Logistic_shippingType>? ShippingTypes { get; set; } = new List<Logistic_shippingType>();
    }
}
