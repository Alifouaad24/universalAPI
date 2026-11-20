using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class ShippingCost
    {
        [Key]
        public int ShippingCostId { get; set; }
        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }

        public Weight_Unit? Weight_Unit { get; set; }
        [ForeignKey(nameof(Weight_Unit))]
        public int? Weight_UnitId { get; set; }

        public decimal? Weight_Value { get; set; }


        public Logistic? Logistic { get; set; }
        [ForeignKey(nameof(Logistic))]
        public int? LogisticId { get; set; }

        public decimal? CostAmount { get; set; }
        public decimal? SaleAmount { get; set; }


    }
}
