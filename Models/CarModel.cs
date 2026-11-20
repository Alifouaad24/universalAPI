using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class CarModel
    {
        [Key]
        public int CarModelId { get; set; }

        public string CarModelName { get; set; }

        public int CarBrandId { get; set; }

        [ForeignKey(nameof(CarBrandId))]
        public CarBrand? CarBrand { get; set; }
    }
}
