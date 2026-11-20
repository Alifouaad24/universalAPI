using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models.Helper_models
{
    public class ItemDto
    {
        public string? ItemDescription { get; set; }
        public string? ItemDetails { get; set; }
        public string? sku { get; set; }
        public string? InternetId { get; set; }
        public string? upc { get; set; }
        public string? Model { get; set; }
        public int? PlatformId { get; set; }
        public int? BusinessId { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public int? UnitId { get; set; }
        public decimal? UnitValue { get; set; }
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? CategoryId { get; set; }
        public decimal BasePrice { get; set; }
        public int? CurrencyId { get; set; }
        public int? BrandId { get; set; }
        public int? ItemConditionId { get; set; }
        public int? Qty { get; set; }
        public decimal? invPrice { get; set; }
        
        // public List<IFormFile>? Images { get; set; }
        public List<int>? DeletedImageIds { get; set; }

    }

    public class ItemDto1
    {
        public string? ItemDescription { get; set; }
        public string? ItemDetails { get; set; }
        public string? sku { get; set; }
        public string? InternetId { get; set; }
        public string? upc { get; set; }
        public string? Model { get; set; }
        public int? PlatformId { get; set; }
        public int? BusinessId { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public int? UnitId { get; set; }
        public decimal? UnitValue { get; set; }
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? CategoryId { get; set; }
        public decimal BasePrice { get; set; }
        public int? CurrencyId { get; set; }
        public List<IFormFile>? Images { get; set; }
        public int? BrandId { get; set; }

    }

    public class ItemJsonDto
    {
        public string? ItemDescription { get; set; }
        public string? ItemDetails { get; set; }
        public string? Sku { get; set; }
        public string? Brand { get; set; }
        public string? InternetId { get; set; }
        public string? Upc { get; set; }
        public string? Model { get; set; }
        public int? PlatformId { get; set; }
        public int? BusinessId { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public int? UnitId { get; set; }
        public decimal? UnitValue { get; set; }
        public int? ColorId { get; set; }
        public int? SizeId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? BasePrice { get; set; }
        public int? CurrencyId { get; set; }
        public List<string>? Images { get; set; }
    }
}
