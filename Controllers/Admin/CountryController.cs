using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;
using Universal_server.Models.Helper_models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        UniversalDbContext db;
        public CountryController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCountries()
        {
            var Countries = await db.Countries.Where(b => b.visible == true).ToListAsync();
            return Ok(Countries);
        }

        [HttpPost]
        public async Task<IActionResult> AddCountry([FromBody] CountryDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var country = new Country
            {
                Name = model.Name,
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
            };
            await db.Countries.AddAsync(country);
            await db.SaveChangesAsync();
            return Ok(country);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditCountry(int id, [FromBody] CountryDto model)
        {
            var country = await db.Countries.FindAsync(id);
            if (country == null) return NotFound();

            country.Name = model.Name;
            country.Insert_by = "";

            await db.SaveChangesAsync();
            return Ok(country);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
            var country = await db.Countries.FindAsync(id);
            if (country == null) return NotFound();

            country.visible = false;
            await db.SaveChangesAsync();
            return Ok(country);
        }
    }
}
