using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class UniversalOrder
    {
        [Key]
        public int UniversalOrderId { get; set; }

        public Platform? Platform { get; set; }
        [ForeignKey(nameof(Platform))]
        public int? Platform_id { get; set; }

        public Business? Business { get; set; }
        [ForeignKey(nameof(Business))]
        public int? Business_id { get; set; }

        public GlobalCustomer? GlobalCustomer { get; set; }
        [ForeignKey(nameof(GlobalCustomer))]
        public int? GlobalCustomerId { get; set; }

        public ShippingType? ShippingType { get; set; }
        [ForeignKey(nameof(ShippingType))]
        public int? ShippingTypeId { get; set; }

        public UniversalOrderStatus? OrderStatus { get; set; }
        [ForeignKey(nameof(OrderStatus))]
        public int? OrderStatusId { get; set; }

        public List<OrderItems>? OrderItems { get; set; }


        public Supplier? Supplier { get; set; }
        [ForeignKey(nameof(Supplier))]
        public int? SupplierId { get; set; }


        public string Cart_value { get; set; }
        public string CreatedBy { get; set; }
        public string? Modified_by { get; set; }
        public DateOnly CreatedDate { get; set; }
    }
}
