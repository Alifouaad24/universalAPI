using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models.Helper_models
{
    public class GlobalCustomerDto
    {
        public string CustomerName { get; set; }
        public string CustomerMobile { get; set; }
        public string? CustomerEmail { get; set; }
        public string? customerCode { get; set; }

        public List<int>? BusinessesIds { get; set; }
        public int? Country_id { get; set; }
        public AddressDto? Address { get; set; }

    }
}
