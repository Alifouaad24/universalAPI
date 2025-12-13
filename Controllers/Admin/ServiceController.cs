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
            var services = await db.Services.ToListAsync();
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
    }
}
