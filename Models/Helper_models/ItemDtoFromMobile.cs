namespace Universal_server.Models.Helper_models
{
    public class ItemDtoFromMobile
    {
        public string upc { get; set; }
        public int PlatformId { get; set; }
        public List<IFormFile> Images { get; set; }
    }
}
