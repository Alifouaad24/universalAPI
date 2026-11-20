using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Item
    {
        [Key]
        public int ItemId { get; set; }

        public string? ItemDescription { get; set; }
        public string? ItemDetails { get; set; }

        public string? SKU { get; set; } = null!;
        public string? InternetId { get; set; }
        public string? UPC { get; set; }
        public string? ShortCode { get; set; }
        public string? Model { get; set; }

        [ForeignKey(nameof(Platform))]
        public int? PlatformId { get; set; }
        public Platform? Platform { get; set; }

        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }


        [ForeignKey(nameof(Unit))]
        public int? UnitId { get; set; }
        public Unit? Unit { get; set; }
        public decimal? UnitValue { get; set; }
        


        [ForeignKey(nameof(Color))]
        public int? ColorId { get; set; }
        public Color? Color { get; set; }




        ///  /////////////////////////////////////
        public CarBrand? CarBrand { get; set; }
        [ForeignKey(nameof(CarBrand))]
        public int? CarBrandId { get; set; }
        



        public string? Description { get; set; }
        public decimal? Weight { get; set; }
        public string? Internet { get; set; }



        public int? SizeId { get; set; }
        public Size? Size { get; set; }

        [ForeignKey(nameof(Category))]
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public decimal? BasePrice { get; set; }

        [ForeignKey(nameof(Currency))]
        public int? CurrencyId { get; set; }
        public Currency? Currency { get; set; }

        public bool? IsActive { get; set; } = true;
        public bool? IsScraped { get; set; } = false;
        public bool? IsComplated { get; set; } = false;
        public bool? CanScrape { get; set; }

        public ICollection<ItemImage>? Images { get; set; } = new List<ItemImage>();
        public ICollection<BusinessItem>? BusinessItems { get; set; } = new List<BusinessItem>();

        public string? InsertBy { get; set; }

        /////////////////////////////////
        /// 
        public string? VinNumber { get; set; }
        public CarYear? CarYear { get; set; }
        [ForeignKey(nameof(CarYear))]
        public int? CarYearId { get; set; }
        public CarModel? CarModel { get; set; }
        [ForeignKey(nameof(CarModelId))]
        public int? CarModelId { get; set; }
        /// ////////////////////////////
        [JsonIgnore]
        public GlobalCustomer? Customer { get; set; }
        [ForeignKey(nameof(GlobalCustomerId))]
        public int? GlobalCustomerId { get; set; }
    }
}
