using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class CauseController : ControllerBase
    {
        private readonly UniversalDbContext _context;
        public CauseController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCauses()
        {
            var causes = await _context.Causes.ToListAsync();
            return Ok(causes);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewCause([FromBody] CauseDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var cause = new Cause
            {
                CauseName = model.Name,
            };

            await _context.Causes.AddAsync(cause);
            await _context.SaveChangesAsync();
            return Ok(cause);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCause(int id, [FromBody] CauseDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var existCause = await _context.Causes.FindAsync(id);

            existCause.CauseName = model.Name;
            await _context.SaveChangesAsync();

            return Ok(existCause);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCause(int id)
        {
            var existCause = await _context.Causes.FindAsync(id);
            if (existCause == null) return NotFound("Cause not found");
            _context.Causes.Remove(existCause);
            await _context.SaveChangesAsync();

            return Ok(existCause);
        }

    }
}

public class CauseDto
{
    public string Name { get; set; }
}
