using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models.Helper_models
{
    public class AddressDto
    {
        public string Line_1 { get; set; }
        public string Line_2 { get; set; }
        public string State { get; set; }
        public string Post_code { get; set; }
        public string City { get; set; }
    }
}
