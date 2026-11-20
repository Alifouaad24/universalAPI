using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarsInfoController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        public CarsInfoController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetCarInfo(string chaseh)
        {
            string onlychaseh = chaseh.Trim().Replace(" ", "");
            if (onlychaseh.Length <= 0)
                return BadRequest("chaseh length cant be empty");
            var response = await _httpClient.GetAsync(
                $"https://vpic.nhtsa.dot.gov/api/vehicles/decodevinvalues/{onlychaseh}?format=json"
            );

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, "Error calling external API");
            }

            var jsonString = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<CarInfoResponse>(jsonString);

            var car = result?.Results?.FirstOrDefault();

            if (car == null)
                return NotFound("No data");

            return Ok(new
            {
                Make = car.Make ?? "Unknown",
                Model = car.Model ?? "Unknown",
                Year = car.ModelYear ?? "Unknown",
                Body = car.BodyClass ?? "Unknown",
                Cylinders = car.EngineCylinders ?? "Unknown",
                Fuel = car.FuelTypePrimary ?? "Unknown",
                Vin = car.VIN ?? chaseh
            });
        }
    }
}
public class CarInfoResponse
{
    public List<CarInfo> Results { get; set; }
}

public class CarInfo
{
    public string Make { get; set; }
    public string Model { get; set; }
    public string ModelYear { get; set; }
    public string BodyClass { get; set; }
    public string EngineCylinders { get; set; }
    public string FuelTypePrimary { get; set; }
    public string VIN { get; set; }
}