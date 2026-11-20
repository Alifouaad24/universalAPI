namespace Universal_server.Models.Helper_models
{
    public class PlatformDto
    {
        public string Description { get; set; }
        public List<PlatformBus>? PlatformBus { get; set; }


    }


    public class PlatformBus
    {
        public int businessId { get; set; }
        public bool isForScrape { get; set; }
        public bool isForSell { get; set; }


    }
}
