using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class StateController : ControllerBase
    {
        UniversalDbContext db;
        public StateController(UniversalDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStates()
        {
            var states = await db.States.Include(s => s.Country).ToListAsync();
            return Ok(states);
        }

        [HttpGet("GetAllStatesByCountry/{id}")]
        public async Task<IActionResult> GetAllStatesByCountry(int id)
        {
            var states = await db.States.Include(s => s.Country).Where(s => s.CountryId == id).ToListAsync();
            return Ok(states);
        }
    }
}
