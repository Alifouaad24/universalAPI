using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models.Helper_models;
using Universal_server.Models;
using Microsoft.VisualBasic;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinantialItemController : ControllerBase
    {
        UniversalDbContext db;
        public FinantialItemController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var finantials = await db.Finantial_items.ToListAsync();
            return Ok(finantials);
        }

        [HttpPost]
        public async Task<IActionResult> addFinantial([FromBody] FinantialDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var finantial = new Finantial_item
            {
                Finantial_description = model.Description,
            };

            await db.Finantial_items.AddAsync(finantial);
            await db.SaveChangesAsync();

            return Ok(finantial);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> updateFinantial(int id, [FromBody] FinantialDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var finantial = await db.Finantial_items.FindAsync(id);
            if (finantial == null) return BadRequest(ModelState);

            finantial.Finantial_description = model.Description;
            await db.SaveChangesAsync();
            return Ok(finantial);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFinantial(int id)
        {
            var finantial = await db.Finantial_items.FindAsync(id);
            if (finantial == null) return NotFound();

            db.Finantial_items.Remove(finantial);
            await db.SaveChangesAsync();

            return Ok(finantial);
        }

    }
}
