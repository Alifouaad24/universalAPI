using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class PlatformAccount
    {
        public int PlatformAccountId { get; set; }

        public Platform? Platform { get; set; }
        [ForeignKey(nameof(Platform))]
        public int PlatformId { get; set; }

        public string Account { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
        public bool IsActive { get; set; }

    }
}
