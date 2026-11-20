using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogisticController : ControllerBase
    {
        private readonly UniversalDbContext dbContext;
        public LogisticController(UniversalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var logistics = await dbContext.Logistics.Include(l => l.Business).ToListAsync();
            return Ok(logistics);
        }
    }
}
