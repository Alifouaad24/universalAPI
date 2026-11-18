using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Inventory_Published_Items
    {
        [Key]
        public int Inventory_Published_Items_Id {  get; set; }

        public Inventory? Inventory { get; set; }
        [ForeignKey(nameof(Inventory))]
        public int Inventory_id {  get; set; }

        public Platform? Platform { get; set; }
        [ForeignKey(nameof(Platform))]
        public int Platform_id {  get; set; }

        public DateOnly? PublishedOn { get; set; }
    }
}
