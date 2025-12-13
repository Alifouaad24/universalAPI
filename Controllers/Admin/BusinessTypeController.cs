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
    public class BusinessTypeController : ControllerBase
    {
        UniversalDbContext db;
        public BusinessTypeController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> getAllBusinessesType()
        {
            var bus_types = await db.Business_types.Where(b => b.visible == true).ToListAsync();
            return Ok(bus_types);
        }


        [HttpPost]
        public async Task<IActionResult> AddBusinessType([FromBody] BusinessTypeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var businessType = new Business_type
            {
                Description = model.Description,
            };

            await db.Business_types.AddAsync(businessType);
            await db.SaveChangesAsync();
            return Ok(businessType);
        }
    }
}
