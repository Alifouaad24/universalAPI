using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models
{
    public class Currency
    {
        [Key]
        public int CurrencyId { get; set; }
        public string Code { get; set; } = null!;

    }
}

