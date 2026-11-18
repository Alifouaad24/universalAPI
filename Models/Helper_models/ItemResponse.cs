namespace Universal_server.Models.Helper_models
{
    public class ItemResponse
    {
        public int ItemId { get; set; }

        public string? ItemDescription { get; set; }
        public string? ItemDetails { get; set; }
        public string? SKU { get; set; }
        public string? InternetId { get; set; }
        public string? UPC { get; set; }
        public string? ShortCode { get; set; }
        public string? Model { get; set; }

        public int? PlatformId { get; set; }
        public Platform? Platform { get; set; }

        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }

        public int? UnitId { get; set; }
        public Unit? Unit { get; set; }
        public decimal? UnitValue { get; set; }

        public int? ColorId { get; set; }
        public Color? Color { get; set; }

        public int? CarBrandId { get; set; }
        public CarBrand? CarBrand { get; set; }

        public string? Description { get; set; }
        public decimal? Weight { get; set; }
        public string? Internet { get; set; }

        public int? SizeId { get; set; }
        public Size? Size { get; set; }

        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public decimal? BasePrice { get; set; }

        public int? CurrencyId { get; set; }
        public Currency? Currency { get; set; }

        public bool? IsActive { get; set; }
        public bool? IsScraped { get; set; }
        public bool? IsComplated { get; set; }
        public bool? CanScrape { get; set; }

        public ICollection<ItemImage>? Images { get; set; }
        public ICollection<BusinessItem>? BusinessItems { get; set; }

        public string? InsertBy { get; set; }

        public string? VinNumber { get; set; }

        public int? CarYearId { get; set; }
        public CarYear? CarYear { get; set; }

        public int? CarModelId { get; set; }
        public CarModel? CarModel { get; set; }

        public int? GlobalCustomerId { get; set; }

        // الحقل الإضافي، ليس في قاعدة البيانات
        public bool IsInStock { get; set; }
    }
}
