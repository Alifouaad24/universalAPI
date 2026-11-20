using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class ItemCondition
    {
        [Key]
        public int ItemConditionId { get; set; }
        public string Description { get; set; }
        public int? EbayConditionId { get; set; }
    }
}
