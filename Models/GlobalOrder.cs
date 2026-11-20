using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class GlobalOrder
    {
        [Key]
        public int GlobalOrderId { get; set; }

        public GlobalCustomer? Customer { get; set; }
        [ForeignKey(nameof(Customer))]
        public int? GlobalCustomerId { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int Business_id { get; set; }

        public Item? Item { get; set; }
        [ForeignKey(nameof(Item))]
        public int? ItemId { get; set; }
        public DateOnly InsertDate { get; set; }
        public DateOnly? Schedule_dt { get; set; }
        public string? Schedule_time { get; set; }
        public string? Notes { get; set; }
        public List<caseImage>? orderImages { get; set; }


        //////////////////////////////////////////////////////////////////////////////

        public Platform? Platform { get; set; }
        [ForeignKey(nameof(Platform))]
        public int? Platform_id { get; set; }
        public ShippingType? ShippingType { get; set; }
        [ForeignKey(nameof(ShippingType))]
        public int? ShippingTypeId { get; set; }


        public Service? Service { get; set; }
        [ForeignKey(nameof(Service))]
        public int? Service_id { get; set; }
        public UniversalOrderStatus? OrderStatus { get; set; }
        [ForeignKey(nameof(OrderStatus))]
        public int? OrderStatusId { get; set; }


        // if the order goods
        // public List<OrderItems>? OrderItems { get; set; }


        public List<GlobalOrderDetail>? GlobalOrderDetail { get; set; }


        public string? Cart_value { get; set; }
        public string? CreatedBy { get; set; } // new 14/8/2026
        public string? Modified_by { get; set; }
        public DateOnly CreatedDate { get; set; }

        //////////////////////////////////////////////////////// 
        ///

        public Business? ConsumerBusiness { get; set; }
        [ForeignKey(nameof(ConsumerBusiness))]
        public int? ConsumerBusiness_id { get; set; }


        public IdentityUserData? IdentityUserData { get; set; }
        [ForeignKey(nameof(IdentityUserData))]
        public string? IdentityUserDataId { get; set; }

        // //////////////////////////////////

        public List<Comment>? Comments { get; set; }

        ///////////////////////////////////////  
        ///

        public string? AssignerId { get; set; }
        public Assign_type? AssignerType { get; set; }
        [ForeignKey(nameof(AssignerType))]
        public int? AssignerTypeId { get; set; }



        public string? AssigneeId { get; set; }
        public Assign_type? AssigneeType { get; set; }
        [ForeignKey(nameof(AssigneeType))]
        public int? AssigneeTypeId { get; set; }

        ///////////////////////Supplier/////////////////////////////////
        public Supplier? Supplier { get; set; }
        [ForeignKey(nameof(Supplier))]
        public int? SupplierId { get; set; }

        public List<OrderItems>? OrderItems { get; set; }
    }
}
