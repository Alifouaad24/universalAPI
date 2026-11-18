using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models.AinAlFHDModels
{
    [Table("Platforms")]
    public class AinAlFhdPlatform
    {
        [Key]
        public int Platform_id { get; set; }

        public string? Desciption { get; set; }
    }
}
