using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Inventory_business
    {
        [Key]
        public int Inventory_business_id { get; set; }
        public int Qty { get; set; }

        public Inventory? Inventory { get; set; }
        [ForeignKey(nameof(Inventory))]
        public int Inventory_id { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int Business_id { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;



    }
}
