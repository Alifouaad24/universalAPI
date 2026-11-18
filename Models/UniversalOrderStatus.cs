using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class UniversalOrderStatus
    {
        [Key]
        public int OrderStatusId { get; set; }
        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }

        public Service? Service { get; set; }
        [ForeignKey(nameof(Service))]
        public int? Service_id { get; set; }
        public string StatusAr { get; set; }
        public string StatusEn { get; set; }
    }
}
