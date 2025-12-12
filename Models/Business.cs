using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Business
    {
        [Key]
        public int Business_id { get; set; }

        [Required]
        public string Business_name { get; set; }


        [ForeignKey(nameof(Country))]
        public int CountryId { get; set; }
        public Country? Country { get; set; }

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

        public List<Activiity>? Activities { get; set; }

        public List<Business_BusinessType>? BusinessTypes { get; set; }
        public List<Business_Address>? BusinessAddresses { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }

}
