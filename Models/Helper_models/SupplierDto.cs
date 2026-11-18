using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models.Helper_models
{
    public class SupplierDto
    {
        public int? ProviderId { get; set; }
        public int? ConsumerId { get; set; }
        public int? ServiceId { get; set; }
        public int? Platform_id { get; set; }
    }
}
