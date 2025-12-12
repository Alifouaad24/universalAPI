using System.ComponentModel.DataAnnotations.Schema;

namespace Universal_server.Models
{
    public class Business_Address
    {
        public int Business_id { get; set; }
        public Business Business { get; set; }

        public int Address_id { get; set; }
        public Address Address { get; set; }


        public DateOnly Insert_on { get; set; }
        public string? Insert_by { get; set; }
        public bool visible { get; set; } = true;
    }
}
