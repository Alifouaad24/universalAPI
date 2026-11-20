using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : ControllerBase
    {
        UniversalDbContext db;
        public SizeController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllData()
        {
            var data = await db.Sizes.ToListAsync();
            return Ok(data);
        }
    }
}
