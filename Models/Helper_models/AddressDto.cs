using System.ComponentModel.DataAnnotations;

namespace Universal_server.Models.Helper_models
{
    public class AddressDto
    {
        public string? Line_1 { get; set; }
        public string? Line_2 { get; set; }
        public string? Us_City { get; set; }
        public string? LandMark { get; set; }
        public string? Post_code { get; set; }


        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public int? AreaId { get; set; }
        public int? CountryId { get; set; }
    }
}
