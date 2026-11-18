namespace Universal_server.Models.Helper_models
{
    public class UserImagesDto
    {
        public List<IFormFile> Images { get; set; }
        public int BusinessId { get; set; }
        public int ItemId { get; set; }
        public int FolderId { get; set; }
        public bool AddToItemAndInventory { get; set; } = false;
        public string? upc { get; set; }

        public int? CategoryId { get; set; }
        public int? PlatformId { get; set; }
        public int? ItemConditionId { get; set; }
        public string? Description { get; set; }
        public string? Details { get; set; }
        public decimal? ItemPrice { get; set; }
        public decimal? WarehousePrice { get; set; }
    }
}
