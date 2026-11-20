using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemConditionController : ControllerBase
    {
        UniversalDbContext _context;
        public ItemConditionController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var conditions = await _context.ItemConditions.ToListAsync();
            return Ok(conditions);
        }
    }
}
