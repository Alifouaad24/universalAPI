using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Universal_server.Data;
using Universal_server.Models;

namespace Universal_server.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    public class GlobalShippingTypesController : ControllerBase
    {
        private readonly UniversalDbContext _context;
        public GlobalShippingTypesController(UniversalDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllShippingTypes()
        {
            var shippings =await _context.ShippingTypes.ToListAsync();
            return Ok(shippings);
        }
    }
}
