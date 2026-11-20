using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class Category
    {
        [Key]
        public int Category_id { get; set; }
        public int? ParentId { get; set; }
        public int? LinkedCategoryId { get; set; }
        [Required]
        public string Name { get; set; }
        public string? EbayCategoryId { get; set; }
        [JsonIgnore]
        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }
       
        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
