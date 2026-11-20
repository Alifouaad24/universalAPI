using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeightUnitController : ControllerBase
    {
        private readonly UniversalDbContext dbContext;
        public WeightUnitController(UniversalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var units = await dbContext.Weight_Units.ToListAsync();
            return Ok(units);
        }
    }
}
