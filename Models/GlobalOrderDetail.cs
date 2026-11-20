using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class GlobalOrderDetail
    {
        [Key]
        public int GlobalOrderDetailId { get; set; }

        public Service? Service { get; set; }
        [ForeignKey(nameof(Service))]
        public int? Service_id { get; set; }

        public UniversalOrderStatus? OrderStatus { get; set; }
        [ForeignKey(nameof(OrderStatus))]
        public int? ServiceStatusId { get; set; }

        public Item? Item { get; set; }
        [ForeignKey(nameof(Item))]
        public int? ItemId { get; set; }


        [JsonIgnore]
        public GlobalOrder? GlobalOrder { get; set; }
        [ForeignKey(nameof(GlobalOrder))]
        public int? GlobalOrderId { get; set; }



        public string? Notes { get; set; }
        public decimal? Cost { get; set; }
        public decimal? Discount { get; set; }
        public decimal? paid { get; set; }

        public decimal? PriceDollar { get; set; }
        public decimal? PriceIQ { get; set; }
        public string? ImageUrl { get; set; }



        // assign to
        public IdentityUserData? IdentityUserData { get; set; }
        [ForeignKey(nameof(IdentityUserData))]
        public string? IdentityUserDataId { get; set; }



        public List<Case_Service_Notes>? Case_Service_Notes { get; set; }

        //// ////////////////////////////////////////
        ///

        public Complaint? Complaint { get; set; }
        [ForeignKey(nameof(Complaint))]
        public int? ComplaintId { get; set; }


        public Cause? Cause { get; set; }
        [ForeignKey(nameof(Cause))]
        public int? CauseId { get; set; }
        /////
        ///
        [ForeignKey(nameof(Unit))]
        public int? UnitId { get; set; }
        public Unit? Unit { get; set; }
        public decimal? UnitValue { get; set; }

    }
}
