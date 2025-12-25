using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        UniversalDbContext db;
        public CityController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCities()
        {
            var cities = await db.Cities.Include(c => c.Areas).Include(c => c.Country).ToListAsync();
            return Ok(cities);
        }

        [HttpGet("GetAllCitiesByCountry/{id}")]
        public async Task<IActionResult> GetAllCitiesByCountry(int id)
        {
            var cities = await db.Cities.Include(c => c.Areas).Include(c => c.Country).Where(c => c.CountryId == id).ToListAsync();
            return Ok(cities);
        }
    }
}
