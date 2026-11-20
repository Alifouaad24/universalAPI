using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitController : ControllerBase
    {
        UniversalDbContext db;
        public UnitController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllData()
        {
            var data = await db.Units.ToListAsync();
            return Ok(data);
        }
    }
}
