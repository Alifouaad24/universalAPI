using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Multimedia
    {
        [Key]
        public int MultimediaId { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int Business_id { get; set; }
        public string fileUrl { get; set; }
    }
}
