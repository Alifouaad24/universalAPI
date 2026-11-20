using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Universal_server.Models
{
    public class GlobalCustomer
    {
        [Key]
        public int GlobalCustomerId { get; set; }
        [Required]
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string? CustomerEmail { get; set; }
        public string? customerCode { get; set; }
        public string? CustomerImage { get; set; }

        [ForeignKey(nameof(Country))]
        public int? Country_id { get; set; }
        public Country? Country { get; set; }

        public List<Address>? Address { get; set; }

        public List<Buseness_Customer>? Buseness_Customers { get; set; }
        public List<System_Business_customer>? System_Business_customers { get; set; }
        [JsonIgnore]
        public List<GlobalOrder>? GlobalOrders { get; set; }
        [JsonIgnore]
        public List<CarInfoTbl>? CarsInfoTbl { get; set; }
        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;

    }
}
