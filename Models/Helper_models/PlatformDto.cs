namespace Universal_server.Models.Helper_models
{
    public class PlatformDto
    {
        public string Description { get; set; }
        public string? AccountEmail { get; set; }
        public string? AccountName { get; set; }
        public string userId { get; set; }
        public bool Chat { get; set; }
        public bool Scrape { get; set; }
        public bool Post { get; set; }
        public bool Ads { get; set; }
        public bool Account { get; set; }
        public List<PlatformBus>? PlatformBus { get; set; }


    }


    public class PlatformBus
    {
        public int businessId { get; set; }
        public bool isForScrape { get; set; }
        public bool isForSell { get; set; }
    }
}
