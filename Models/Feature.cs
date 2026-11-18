using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Feature
    {
        [Key]
        public int FeatureId { get; set; }
        public string? Body { get; set; }
        public string? Status { get; set; }
        public List<Comment>? Comments { get; set; }


        public GlobalSystem? GlobalSystem { get; set; }
        [ForeignKey(nameof(GlobalSystem))]
        public int? GlobalSystemId { get; set; }

        public Business? Business { get; set; }

        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }


        public Service? Service { get; set; }

        [ForeignKey(nameof(Service))]
        public int? Service_id { get; set; }

        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
