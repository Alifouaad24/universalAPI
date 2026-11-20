using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierId { get; set; }

        public Business? ProviderBusiness { get; set; }
        [ForeignKey(nameof(ProviderBusiness))]
        public int? ProviderId { get; set; }


        public Business? ConsumerBusiness { get; set; }
        [ForeignKey(nameof(ConsumerBusiness))]
        public int? ConsumerId { get; set; }

        public Service? Service { get; set; }
        [ForeignKey(nameof(Service))]
        public int? ServiceId { get; set; }

        public Platform? Platform { get; set; }
        [ForeignKey(nameof(Platform))]
        public int? Platform_id { get; set; }


    }
}
