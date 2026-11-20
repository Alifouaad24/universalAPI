using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class UserImage
    {
        [Key]
        public int UserImagesId { get; set; }
        [Required]
        public string UserId { get; set; }
        public int FolderId { get; set; }

        // after proccess and added to item tbl
        public Item? Item { get; set; }
        [ForeignKey(nameof(Item))]
        public int? ItemId { get; set; }

        public int BusinessId { get; set; }
        [Required]
        public string ImageUrl { get; set; }
        public DateTime InsertDate { get; set; }

        public bool IsProccessed { get; set; } = false;
    }
}
