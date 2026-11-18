using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class CarImage
    {
        [Key]
        public int CarImageId { get; set; }

        public CarInfoTbl CarInfoTbl { get; set; } = null!;
        [ForeignKey(nameof(CarInfoTbl))]
        public int CarInfoTblId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = null!;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
