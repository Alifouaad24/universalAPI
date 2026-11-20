using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class BusinessGroup
    {
        [Key]
        public int BusinessGroupId { get; set; }
        [Required]
        public String GroupName { get; set; }
        public String UserId { get; set; }
        public DateOnly CreatedAt { get; set; }

        public List<BusinessGroupBusinesses>? BusinessGroupBusinesses { get; set; }
    }
}
