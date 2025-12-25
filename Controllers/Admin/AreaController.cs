using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        UniversalDbContext db;
        public AreaController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAreas()
        {
            var areas = await db.Areas.Include(a => a.City).ThenInclude(c => c.Country).ToListAsync();
            return Ok(areas);
        }

        [HttpGet("GetAlAreasByCity/{id}")]
        public async Task<IActionResult> GetAlAreasByCity(int id)
        {
            var areas = await db.Areas.Include(a => a.City).ThenInclude(c => c.Country).Where(a => a.CityId == id).ToListAsync();
            return Ok(areas);
        }
    }
}
