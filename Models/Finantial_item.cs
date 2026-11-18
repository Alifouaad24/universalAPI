using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Finantial_item
    {
        [Key]
        public int Finantial_itemId { get; set; }
        [Required]
        public string Finantial_description { get; set; }
    }
}
