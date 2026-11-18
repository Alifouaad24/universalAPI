using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Universal_server.Models;

public class CarBrand
{
    [Key]
    public int CarBrandId { get; set; }
    public int? BusinessId { get; set; }

    public string CarBrandName { get; set; }

    public string? CarBrandImgUrl { get; set; }
    [JsonIgnore]
    public ICollection<CarModel>? CarModels { get; set; }
}