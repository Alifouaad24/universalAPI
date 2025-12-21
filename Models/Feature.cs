using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Feature
    {
        [Key]
        public int FeatureId { get; set; }
        public string Name { get; set; }

        [ForeignKey(nameof(Service))]
        public int? Service_id {  get; set; }
        public Service? Service { get; set; }
    }
}
