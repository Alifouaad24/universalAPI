using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models.Helper_models
{
    public class BusinessDTO
    {
        public string Business_name { get; set; }
        public int CountryId { get; set; }
        public bool Is_active { get; set; }
        public string? Business_phone { get; set; }
        public string? Business_webSite { get; set; }
        public string? Business_fb { get; set; }
        public string? Business_instgram { get; set; }
        public string? Business_tiktok { get; set; }
        public string? Business_google { get; set; }
        public string? Business_youtube { get; set; }
        public string? Business_whatsapp { get; set; }
        public string? Business_email { get; set; }

        public int BusinessTypeId { get; set; }
        public int AddressId { get; set; }

    }
}
