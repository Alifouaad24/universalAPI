using System.Diagnostics;
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
    public class ServiceController : ControllerBase
    {
        UniversalDbContext db;
        public ServiceController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllServices() 
        {
            var services = await db.Services.Where(b => b.visible == true).ToListAsync();
            return Ok(services);
        }

        [HttpPost]
        public async Task<IActionResult> addService([FromBody] ServiceDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var service = new Service
            {
                Description = model.Description,
                Activity_id = model.Activity_id,
                Insert_by = "",
                Insert_on = DateOnly.FromDateTime(DateTime.Now),
                visible = true,
            };
            await db.Services.AddAsync(service);
            await db.SaveChangesAsync();
            return Ok(service);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditService(int id, [FromBody] ServiceDto model)
        {
            var service = await db.Services.FindAsync(id);
            if (service == null) return NotFound();

            service.Description = model.Description;
            service.Activity_id = model.Activity_id;
            service.Insert_by = "";

            await db.SaveChangesAsync();
            return Ok(service);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            var service = await db.Services.FindAsync(id);
            if (service == null) return NotFound();

            service.visible = false;
            await db.SaveChangesAsync();
            return Ok(service);
        }
    }
}
