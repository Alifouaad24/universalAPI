using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models.Helper_models;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeatureController : ControllerBase
    {
        UniversalDbContext db;
        public FeatureController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var features = await db.Features.Include(b => b.Service).ToListAsync();
            return Ok(features);
        }


        [HttpPost]
        public async Task<IActionResult> addService([FromBody] FeatureDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var feature = new Feature
            {
                Name = model.Name,
                Service_id = model.ServiceId,
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
            };

            await db.Features.AddAsync(feature);
            await db.SaveChangesAsync();

           
            return Ok(feature);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeature(int id)
        {
            var feature = await db.Features.FindAsync(id);
            if (feature == null) return NotFound();

            feature.visible = false;
            await db.SaveChangesAsync();
            return Ok(feature);
        }
    }
}
