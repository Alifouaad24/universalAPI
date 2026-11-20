using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models.Helper_models
{
    public class TaareffDto
    {
        public int? PlatformId { get; set; }
        public int? TariffType { get; set; }
        public bool IsActive { get; set; }
    }
}
