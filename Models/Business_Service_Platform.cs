using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Business_Service_Platform
    {
        [Key]
        public int Business_Service_PlatformId { get; set; }

        public Business_Service? Business_Service { get; set; }
        [ForeignKey(nameof(Business_Service))]
        public int Business_ServiceId { get; set; }

        public Platform? Platform { get; set; }
        [ForeignKey(nameof(Platform))]
        public int? Platform_id { get; set; }

    }
}
