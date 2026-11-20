using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class CarInfoTbl
    {
        [Key]
        public int CarInfoTblId { get; set; }
        public string? VinNumber { get; set; }


        public CarYear? CarYear { get; set; }
        [ForeignKey(nameof(CarYear))]
        public int? CarYearId { get; set; }

        /// ////////////////////////////
        [JsonIgnore]
        public GlobalCustomer? Customer { get; set; }
        [ForeignKey(nameof(GlobalCustomerId))]
        public int GlobalCustomerId { get; set; }
        /// ////////////////////////////
        public CarBrand? CarBrand { get; set; }
        [ForeignKey(nameof(CarBrandId))]
        public int? CarBrandId { get; set; }        
        /// ////////////////////////////
        public CarModel? CarModel { get; set; }
        [ForeignKey(nameof(CarModelId))]
        public int? CarModelId { get; set; }

        public List<CarImage>? Images { get; set; }
    }
}
