using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models.Helper_models
{
    public class AssetBusinessDto
    {
        public int? AssetId { get; set; }
        public int? Business_id { get; set; }
        public string? Account_Id { get; set; }
    }
}
