using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class UserFolder
    {
        [Key]
        public int UserFolderId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? ItemId { get; set; }
    }
}
